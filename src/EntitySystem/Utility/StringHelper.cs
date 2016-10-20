using System;
using System.Collections.Generic;
using System.Text;

namespace EntitySystem.Utility
{
	public static class StringHelper
	{
		public static StringBuilder AppendAll<T>(
			this StringBuilder builder, IEnumerable<T> enumerable)
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));
			if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
			foreach (var obj in enumerable) builder.Append(obj);
			return builder;
		}
		
		public static StringBuilder AppendAll<T>(
			this StringBuilder builder, IEnumerable<T> enumerable, string separator)
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));
			if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
			if (separator == null) throw new ArgumentNullException(nameof(separator));
			
			var isFirst = true;
			var previous = default(T);
			foreach (var obj in enumerable) {
				if (isFirst) isFirst = false;
				else builder.Append(previous).Append(separator);
				previous = obj;
			}
			if (!isFirst) builder.Append(previous);
			return builder;
		}
	}
}
