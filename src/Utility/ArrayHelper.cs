using System;
using System.Collections.Generic;

namespace EntitySystem.Utility
{
	public static class ArrayHelper
	{
		public static T[] Fill<T>(T value, int length) =>
			Fill(new T[length], value);
		public static T[] Fill<T>(this T[] array, T value) =>
			Fill(array, value, 0, (array?.Length ?? 0));
		public static T[] Fill<T>(this T[] array, T value, int index, int length)
		{
			ThrowIf.Argument.IsInvalidArrayRange(array, index, length);
			var end = index + length;
			for (var i = index; i < end; i++)
				array[i] = value;
			return array;
		}
		
		public static T[] Clear<T>(this T[] array) =>
			Clear(array, 0, array.Length);
		public static T[] Clear<T>(this T[] array, int index, int length)
			{ Array.Clear(array, index, length); return array; }
		
		public static T[] Resize<T>(this T[] array, int length)
		{
			ThrowIf.Argument.IsNull(array, nameof(array));
			if (length < 0) throw new ArgumentException(
				$"Length ({ length }) is negative", nameof(length));
			var newArray = new T[length];
			Array.Copy(array, newArray, Math.Min(array.Length, length));
			return newArray;
		}
		
		public static T[] CopyFrom<T>(this T[] array, int index, int size, IEnumerable<T> enumerable)
		{
			CopyToCheck(array, 0, size);
			foreach (var item in enumerable)
				array[index++] = item;
			return array;
		}
		
		public static void CopyToCheck(Array array, int index, int size)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));
			if ((index < 0) || (index > array.Length))
				throw new ArgumentOutOfRangeException(nameof(index),
					"Index is outside the specified array");
			if (array.Length - index < size)
				throw new ArgumentException(
					$"The specified array (Length: { array.Length }) is not large " +
					$"enough to hold all elements of this collection (Size: { size })", nameof(array));
		}
	}
}
