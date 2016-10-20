using System;
using System.Collections.Generic;

namespace EntitySystem.Utility
{
	public static class CollectionHelper
	{
		public static Option<TValue> GetOption<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key)
		{
			if (dict == null) throw new ArgumentNullException(nameof(dict));
			TValue value;
			return (dict.TryGetValue(key, out value)
				? Option<TValue>.Some(value)
				: Option<TValue>.None);
		}
		
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, TValue @default = default(TValue)) =>
			GetOption(dict, key).Or(@default);
		
		public static TValue GetOrDefault<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> defaultFactory) =>
			GetOption(dict, key).Or(() => defaultFactory(key));
		
		public static TValue GetOrAdd<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addFactory) =>
			GetOption(dict, key).Or(() => {
				var value = addFactory(key);
				dict.Add(key, value);
				return value;
			});
		
		public static TValue? GetOrNullable<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key)
			where TValue : struct
		{
			if (dict == null) throw new ArgumentNullException(nameof(dict));
			TValue value;
			return (dict.TryGetValue(key, out value) ? value : (TValue?)null);
		}
	}
}
