using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EntitySystem.Utility.Bits
{
	public class BitList : IBitArray, IList<bool>
	{
		uint[] _bits;
		
		public int Count { get; private set; }
		int IBitArray.Length => Count;
		
		public bool this[int index] {
			get {
				if ((index < 0) || (index >= Count))
					throw new ArgumentOutOfRangeException();
				return (((_bits[index >> 5] >> (index & 31)) & 1) != 0);
			}
			set {
				if ((index < 0) || (index >= Count))
					throw new ArgumentOutOfRangeException();
				if (value) _bits[index >> 5] |= (uint)(1 << (index & 31));
				else _bits[index >> 5] &= (uint)~(1 << (index & 31));
			}
		}
		
		
		public BitList(int count, int capacity = -1)
			: this(count, new uint[(Math.Max(count, capacity) + 31) >> 5]) {  }
		public BitList(int count, uint[] bits)
		{
			if (bits == null)
				throw new ArgumentNullException("bits");
			if (count < 0)
				throw new ArgumentException("Count must be zero or positive", "count");
			if (((count + 31) >> 5) > bits.Length)
				throw new ArgumentException(string.Format(
					"Bits is not large enough to contain {0} elements", count), "bits");
			
			_bits = bits;
			Count = count;
		}
		
		public BitList(IEnumerable<bool> bits)
			: this(bits?.ToArray()) {  }
		public BitList(params bool[] bits)
		{
			if (bits == null)
				throw new ArgumentNullException(nameof(bits));
			
			_bits = new uint[(bits.Length + 31) >> 5];
			Count = bits.Length;
			
			for (var i = 0; i < Count; i++)
				this[i] = bits[i];
		}
		
		
		public uint GetUInt(int index)
		{
			if ((index < 0) || (index >= _bits.Length))
				throw new ArgumentOutOfRangeException("index");
			return _bits[index];
		}
		
		
		// IList implementation
		
		public bool IsReadOnly => false;
		
		public void Add(bool bit)
		{
			EnsureCapacity();
			this[Count++] = bit;
		}
		
		public void Insert(int index, bool bit)
		{
			if ((index < 0) || (index > Count))
				throw new ArgumentOutOfRangeException((nameof(index)));
			
			Count++;
			EnsureCapacity();
			
			var i = (index >> 5);
			var last = _bits[i] >> 31;
			
			var mask = (~0U << index);                 //   11110000  (mask)
			_bits[i] = (_bits[i] & ~mask) |            //   ----AAAA
			           ((_bits[i] & mask) << 1);       // | bBBB---- << 1
			                                           // = BBB-AAAA  (b was stored in "last")
			if (bit) _bits[i] |= (1U << (index & 31)); // | ---X----
			for (i++; i < _bits.Length; i++) {
				var first = last;
				last = (_bits[i] >> 31);
				_bits[i] = (_bits[i] << 1) | first;
				// cCCCCCCC << 1  (c stored for next iteration)
				// CCCCCCCb       (b taken from previous iteration)
			}
		}
		
		public void RemoveAt(int index)
		{
			if ((index < 0) || (index >= Count))
				throw new ArgumentOutOfRangeException((nameof(index)));
			
			var i = ((Count-- + 31) >> 5);
			var first = 0U;
			
			for (; i > (index >> 5); i--) {
				var last = first;
				first = (_bits[i] & 1);
				_bits[i] = (_bits[i] >> 1) | (last << 31);
				// BBBBBBBb >> 1  (b stored for next iteration) 
				// aBBBBBBB       (a taken from previous iteration)
			}
			
			var mask = (~0U << index);            //   11110000  (mask)
			_bits[i] = (_bits[i] & ~mask) |       //   ----CCCC
			           ((_bits[i] >> 1) & mask) | // | -DDD~---  (~ is the removed element)
			           (first << 31);             // | b-------  (b taken from last iteration)
		}
		
		public void Clear()
		{
			Count = 0;
			for (var i = 0; i < _bits.Length; i++)
				_bits[i] = 0;
		}
		
		
		int IList<bool>.IndexOf(bool item)
		{
			for (var i = 0; i < Count; i++)
				if (this[i] == item)
					return i;
			return -1;
		}
		
		bool ICollection<bool>.Contains(bool item) =>
			(((IList<bool>)this).IndexOf(item) >= 0);

		bool ICollection<bool>.Remove(bool item)
		{
			var index = ((IList<bool>)this).IndexOf(item);
			if (index >= 0) {
				RemoveAt(index);
				return true;
			} else return false;
		}
		
		void ICollection<bool>.CopyTo(bool[] array, int arrayIndex)
		{
			for (var i = 0; i < Count; i++)
				array[arrayIndex + i] = this[i];
		}
		
		
		// Additional methods
		
		public void RemoveLast()
		{
			if (Count == 0)
				throw new InvalidOperationException("BitList is already empty");
			this[Count--] = false;
		}
		
		void EnsureCapacity()
		{
			var length = ((Count + 31) >> 5);
			if (length <= _bits.Length) return;
			
			var old = _bits;
			_bits = new uint[length];
			Array.Copy(old, _bits, old.Length);
		}
		
		
		// Bitwise operators
		
		public static BitList operator ~(BitList bits)
		{
			if (bits == null)
				throw new ArgumentNullException();
			
			var list = new BitList(bits.Count, bits._bits);
			for (var i = 0; i < list._bits.Length; i++)
				list._bits[i] = ~list._bits[i];
			list._bits[list._bits.Length - 1] &= ~(~0U << (list.Count & 31));
			return list;
		}
		
		public static BitList operator &(BitList left, BitList right) =>
			Combine(left, right, (l, r) => (l & r));
		
		public static BitList operator |(BitList left, BitList right) =>
			Combine(left, right, (l, r) => (l | r));
		
		public static BitList operator ^(BitList left, BitList right) =>
			Combine(left, right, (l, r) => (l ^ r));
		
		static BitList Combine(BitList left, BitList right, Func<uint, uint, uint> func)
		{
			if ((left == null) || (right == null))
				throw new ArgumentNullException();
			if (left.Count != right.Count)
				throw new ArgumentException(string.Format(
					"BitLists are not the same size ({0} != {1})", left.Count, right.Count));
			
			var list = new BitList(left.Count);
			for (var i = 0; i < list._bits.Length; i++)
				list._bits[i] = func(left._bits[i], right._bits[i]);
			return list;
		}
		
		
		public static BitList operator >>(BitList bits, int shift)
		{
			if (bits == null)
				throw new ArgumentNullException();
			if (shift < 0)
				throw new ArgumentOutOfRangeException();
			
			var localShift = (shift & 31);
			var localAntiShift = (32 - localShift);
			
			var list = new BitList(bits.Count);
			for (int i = 0, j = (shift >> 5); (i < ((list.Count - shift) >> 5)) && (j < bits._bits.Length); i++, j++) {
				if (localShift > 0) {
					list._bits[i] = (bits._bits[j] >> localShift);                     // ----AAAA
					if (i > 0) list._bits[i - 1] |= (bits._bits[j] << localAntiShift); // BBBB----
				} else list._bits[i] = bits._bits[j]; // Shortcut in case shift is a multiple of bits in an int.
			}
			return list;
		}
		
		public static BitList operator <<(BitList bits, int shift)
		{
			if (bits == null)
				throw new ArgumentNullException();
			if (shift < 0)
				throw new ArgumentOutOfRangeException();
			
			var localShift = (shift & 31);
			var localAntiShift = (32 - localShift);
			
			var list = new BitList(bits.Count);
			for (int i = (shift >> 5), j = 0; i < list._bits.Length; i++, j++) {
				if (localShift > 0) {
					list._bits[i] = (bits._bits[j] << localShift);                     // AAAA----
					if (j > 0) list._bits[i] |= (bits._bits[j - 1] >> localAntiShift); // ----BBBB
				} else list._bits[i] = bits._bits[j]; // Shortcut in case shift is a multiple of bits in an int.
			}
			return list;
		}
		
		
		// IEnumerable implementation
		
		public IEnumerator<bool> GetEnumerator()
		{
			for (int i = 0, j = 0; i < _bits.Length; i++)
				for (; j < Count; j++)
					yield return ((_bits[i] >> (j & 31)) != 0);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
