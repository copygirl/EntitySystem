using System;
using System.Collections;
using System.Collections.Generic;
using EntitySystem.Utility;

namespace EntitySystem.Collections
{
	public class TypedCollection<T> : IEnumerable<T>
	{
		readonly OptionDictionary<Type, T> _dict;
		
		public TypedCollection() : this(0) {  }
		public TypedCollection(int capacity)
			{ _dict = new OptionDictionary<Type, T>(capacity); }
		
		public TypedCollection(ICollection<T> collection)
			: this(collection?.Count ?? 0)
		{
			ThrowIf.Argument.IsNull(collection, nameof(collection));
			foreach (var item in collection) Add(item);
		}
		
		// Getters
		
		public U Get<U>() where U : T =>
			(U)_dict.Get(typeof(U));
		public Option<U> TryGet<U>() where U : T =>
			_dict.TryGet(typeof(U)).Cast<U>();
		
		public bool Contains<U>() =>
			_dict.ContainsKey(typeof(U));
		
		// Setters
		
		public U GetOrAdd<U>(U item) where U : T =>
			(U)_dict.GetOrAdd(typeof(U), item);
		public U GetOrAdd<U>(Func<U> itemFactory) where U : T =>
			(U)_dict.GetOrAdd(typeof(U), (_) => itemFactory());
		
		public void Add<U>(U item) where U : T =>
			_dict.Add(typeof(U), ThrowIf.Argument.IsNull(item, nameof(item)));
		public Option<U> TryAdd<U>(U item) where U : T =>
			_dict.TryAdd(typeof(U), ThrowIf.Argument.IsNull(item, nameof(item))).Cast<U>();
		
		public void Remove<U>() where U : T =>
			_dict.Remove(typeof(U));
		public Option<U> TryRemove<U>() where U : T =>
			_dict.TryRemove(typeof(U)).Cast<U>();
		
		public Option<U> Set<U>(U item) where U : T =>
			_dict.Set(typeof(U), item).Cast<U>();
		public Option<U> Set<U>(Option<U> item) where U : T =>
			_dict.Set(typeof(U), item.Cast<T>()).Cast<U>();
		
		// IEnumerable implementation
		
		public IEnumerator<T> GetEnumerator() => _dict.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
