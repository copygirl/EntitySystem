using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntitySystem.Utility;

namespace EntitySystem.Collections
{
	public class OptionDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
	{
		static readonly string KEY_EXISTS      = "The specified key is already present in this dictionary";
		static readonly string KEY_NONEXISTANT = "The specified key is not present in this dictionary";
		static readonly string CONCURRENT_MOD  = "The dictionary was modified during enumeration";
		static readonly string KEYS_READONLY   = "KeyCollection is readonly";
		static readonly string VALUES_READONLY = "ValueCollection is readonly";
		
		
		struct Entry
		{
			public int HashCode { get; set; }
			public int Next { get; set; }
			public TKey Key { get; set; }
			public TValue Value { get; set; }
			
			public bool Occupied => (HashCode >= 0);
		}
		
		
		// Members and properties
		
		int[] _buckets;
		Entry[] _entries;
		int _count;
		int _freeList;
		int _freeCount;
		int _version;
		
		KeyCollection _keys = null;
		ValueCollection _values = null;
		
		public IEqualityComparer<TKey> Comparer { get; }
		
		public int Count => (_count - _freeCount);
		
		public KeyCollection Keys =>
			(_keys ?? (_keys = new KeyCollection(this)));
		public ValueCollection Values =>
			(_values ?? (_values = new ValueCollection(this)));
		
		public Option<TValue> this[TKey key] {
			get { return TryGet(key); }
			set { Set(key, value); }
		}
		
		
		// Constructors
		
		public OptionDictionary() : this(0, null) {  }
		public OptionDictionary(int capacity) : this(capacity, null) {  }
		public OptionDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) {  }
		public OptionDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (capacity < 0) throw new ArgumentException(
				"Capacity must be non-negative", nameof(capacity));
			if (capacity > 0) Initialize(capacity);
			Comparer = comparer ?? EqualityComparer<TKey>.Default;
		}
		
		public OptionDictionary(IDictionary<TKey, TValue> dict) : this(dict, null) {  }
		public OptionDictionary(IDictionary<TKey, TValue> dict, IEqualityComparer<TKey> comparer)
			: this(ThrowIf.Argument.IsNull(dict, nameof(dict)).Count, comparer)
			{ foreach (var entry in dict) Add(entry.Key, entry.Value); }
		
		
		// Getters
		
		public TValue Get(TKey key) =>
			TryGet(key).Expect(KeyNonExistant);
		public Option<TValue> TryGet(TKey key)
		{
			bool found; int previous, bucket;
			var index = FindEntry(key, false, out found, out previous, out bucket);
			return (found ? _entries[index].Value : Option<TValue>.None);
		}
		
		public bool ContainsKey(TKey key) =>
			TryGet(key).HasValue;
		
		public bool ContainsValue(TValue value)
		{
			EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
			for (int i = 0; i < _count; i++)
				if (_entries[i].Occupied && comparer.Equals(_entries[i].Value, value))
					return true;
			return false;
		}
		
		// Setters
		
		public TValue GetOrAdd(TKey key, TValue value) =>
			GetOrAdd(key, (_) => value);
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			bool found; int previous, bucket;
			var index = FindEntry(key, true, out found, out previous, out bucket);
			if (!found) _entries[index].Value = valueFactory(key);
			return _entries[index].Value;
		}
		
		public void Add(TKey key, TValue value) =>
			Set(key, Option<TValue>.Some(value), true).ExpectNone(KeyExists);
		public Option<TValue> TryAdd(TKey key, TValue value) =>
			Set(key, Option<TValue>.Some(value), true);
		
		public TValue Remove(TKey key) =>
			Set(key, Option<TValue>.None).Expect(KeyNonExistant);
		public Option<TValue> TryRemove(TKey key) =>
			Set(key, Option<TValue>.None);
		
		public Option<TValue> Set(TKey key, TValue value) => Set(key, Option<TValue>.Some(value), false);
		public Option<TValue> Set(TKey key, Option<TValue> value) => Set(key, value, false);
		private Option<TValue> Set(TKey key, Option<TValue> value, bool abortOnExists)
		{
			bool found; int previous, bucket;
			var index = FindEntry(key, value.HasValue, out found, out previous, out bucket);
			
			Option<TValue> result;
			if (value.HasValue) {
				// Adding or updating an entry.
				
				result = new Option<TValue>(_entries[index].Value, found);
				if (found && abortOnExists) return result;
				_entries[index].Value = (TValue)value;
				
				// If the key was already present, version wasn't increased yet.
				if (found) _version++;
				
			} else if (found) {
				// Removing an existing entry.
				
				result = _entries[index].Value;
				if (previous < 0) _buckets[bucket] = _entries[index].Next;
				else _entries[previous].Next = _entries[index].Next;
				
				_entries[index].HashCode = -1;
				_entries[index].Next = _freeList;
				_entries[index].Key = default(TKey);
				_entries[index].Value = default(TValue);
				
				_freeList = index;
				_freeCount++;
				_version++;
				
			} else result = Option<TValue>.None;
			
			return result;
		}
		
		public void Clear()
		{
			if (_count <= 0) return;
			_buckets.Fill(-1);
			_entries.Clear();
			_freeList  = -1;
			_count     = 0;
			_freeCount = 0;
			_version++;
		}
		
		
		// Utility functions
		
		void Initialize(int capacity)
		{
			var size  = HashHelper.GetPrime(capacity);
			_buckets  = ArrayHelper.Fill(-1, size);
			_entries  = new Entry[size];
			_freeList = -1;
		}
		
		void Resize() =>
			Resize(HashHelper.ExpandPrime(_count));
		void Resize(int newSize)
		{
			Debug.Assert(newSize >= _entries.Length);
			_buckets = ArrayHelper.Fill(-1, newSize);
			_entries = _entries.Resize(newSize);
			
			for (var i = 0; i < _count; i++)
				if (_entries[i].Occupied) {
					var bucket = (_entries[i].HashCode % newSize);
					_entries[i].Next = _buckets[bucket];
					_buckets[bucket] = i;
				}
		}
		
		int FindEntry(TKey key, bool create, out bool found,
		              out int previous, out int bucket)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			
			int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
			found    = false;
			previous = -1;
			bucket   = -1;
			
			// Search through existing buckets.
			if (_buckets != null) {
				bucket = (hashCode % _buckets.Length);
				for (var i = _buckets[bucket]; i >= 0; previous = i, i = _entries[i].Next)
					if ((_entries[i].HashCode == hashCode) && Comparer.Equals(_entries[i].Key, key))
						{ found = true; return i; }
				// If we didn't find the matching key and we
				// don't want to create a new entry, return.
				if (!create) return -1;
			// If no buckets and we want to create, initialize.
			} else if (create) {
				Initialize(0);
				bucket = (hashCode % _buckets.Length);
			// Otherwise bail out!
			} else return -1;
			
			int index;
			if (_freeCount > 0) {
				index = _freeList;
				_freeList = _entries[index].Next;
				_freeCount--;
			} else {
				if (_count == _entries.Length) {
					Resize();
					bucket = (hashCode % _buckets.Length);
				}
				index = _count;
				_count++;
			}
			
			_entries[index].HashCode = hashCode;
			_entries[index].Next = _buckets[bucket];
			_entries[index].Key = key;
			_buckets[bucket] = index;
			_version++;
			
			return index;
		}
		
		IEnumerable<int> FindOccupiedEntries()
		{
			var version = _version;
			for (var i = 0; i < _count; i++) {
				if (_version != version)
					throw new InvalidOperationException(CONCURRENT_MOD);
				if (_entries[i].Occupied)
					yield return i;
			}
		}
		
		static Exception KeyNonExistant() =>
			new ArgumentException(KEY_NONEXISTANT, "key");
		static Exception KeyExists() =>
			new ArgumentException(KEY_EXISTS, "key");
		
		
		// IDictionary implementation
		
		ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
		ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
		
		TValue IDictionary<TKey, TValue>.this[TKey key] {
			get { return Get(key); }
			set { Set(key, value); }
		}
		
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);
		bool IDictionary<TKey, TValue>.Remove(TKey key) => Set(key, Option<TValue>.None).HasValue;
		
		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			var result = TryGet(key);
			value = result.OrDefault();
			return result.HasValue;
		}
		
		// ICollection<KeyValuePair> implementation
		
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			bool found; int previous, bucket;
			var index = FindEntry(item.Key, false, out found, out previous, out bucket);
			return (found && EqualityComparer<TValue>.Default.Equals(item.Value, _entries[index].Value));
		}
		
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
			Add(item.Key, item.Value);
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			var contains = ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item);
			Remove(item.Key);
			return contains;
		}
		
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) =>
			array.CopyFrom(index, Count, (IEnumerable<KeyValuePair<TKey, TValue>>)this);
		
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
			FindOccupiedEntries().Select(
				(i) => new KeyValuePair<TKey, TValue>(
					_entries[i].Key, _entries[i].Value)).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() =>
			((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		
		
		// IReadOnlyDictionary implementation
		
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
		
		TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
			((IDictionary<TKey, TValue>)this)[key];
		
		bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) =>
			((IDictionary<TKey, TValue>)this).TryGetValue(key, out value);
		
		
		// Key/Value collection classes
		
		public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
		{
			readonly OptionDictionary<TKey, TValue> _dict;
			
			public int Count => _dict.Count;
			
			internal KeyCollection(OptionDictionary<TKey, TValue> dict) { _dict = dict; }
			
			public bool Contains(TKey key) => _dict.ContainsKey(key);
			
			// ICollection implementation
			
			bool ICollection<TKey>.IsReadOnly => true;
			
			void ICollection<TKey>.Add(TKey key) { throw new NotSupportedException(KEYS_READONLY); }
			void ICollection<TKey>.Clear() { throw new NotSupportedException(KEYS_READONLY); }
			bool ICollection<TKey>.Remove(TKey key) { throw new NotSupportedException(KEYS_READONLY); }
			
			void ICollection<TKey>.CopyTo(TKey[] array, int index) => array.CopyFrom(index, Count, this);
			
			// IEnumerator implementation
			
			public IEnumerator<TKey> GetEnumerator() =>
				_dict.FindOccupiedEntries().Select(
					(i) => _dict._entries[i].Key).GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		
		public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
		{
			readonly OptionDictionary<TKey, TValue> _dict;
			
			public int Count => _dict.Count;
			
			internal ValueCollection(OptionDictionary<TKey, TValue> dict) { _dict = dict; }
			
			public bool Contains(TValue value) => _dict.ContainsValue(value);
			
			// ICollection implementation
			
			bool ICollection<TValue>.IsReadOnly => true;
			
			void ICollection<TValue>.Add(TValue value) { throw new NotSupportedException(VALUES_READONLY); }
			void ICollection<TValue>.Clear() { throw new NotSupportedException(VALUES_READONLY); }
			bool ICollection<TValue>.Remove(TValue value) { throw new NotSupportedException(VALUES_READONLY); }
			
			void ICollection<TValue>.CopyTo(TValue[] array, int index) => array.CopyFrom(index, Count, this);
			
			// IEnumerator implementation
			
			public IEnumerator<TValue> GetEnumerator() =>
				_dict.FindOccupiedEntries().Select(
					(i) => _dict._entries[i].Value).GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		
		
		// Other helper classes
		
		static class HashHelper
		{
			public const int MAX_PRIME_ARRAY_LENGTH = 0x7FEFFFFD;
			public const int HASH_PRIME = 101;
			public static readonly int[] PRIMES = {
				3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
				1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
				17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
				187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
				1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
			};
			
			public static bool IsPrime(int candidate)
			{
				if ((candidate & 1) != 0) {
					int limit = (int)Math.Sqrt(candidate);
					for (int divisor = 3; divisor <= limit; divisor += 2)
						if ((candidate % divisor) == 0)
							return false;
					return true;
				}
				return (candidate == 2);
			}
			
			public static int GetPrime(int min)
			{
				Debug.Assert(min >= 0);
				for (int i = 0; i < PRIMES.Length; i++) {
					int prime = PRIMES[i];
					if (prime >= min) return prime;
				}
				for (int i = (min | 1); i < Int32.MaxValue; i += 2)
					if (IsPrime(i) && ((i - 1) % HASH_PRIME != 0))
						return i;
				return min;
			}
			
			public static int ExpandPrime(int oldSize)
			{
				int newSize = 2 * oldSize;
				return (((uint)newSize > MAX_PRIME_ARRAY_LENGTH && MAX_PRIME_ARRAY_LENGTH > oldSize)
					? MAX_PRIME_ARRAY_LENGTH
					: GetPrime(newSize));
			}
		}
	}
}
