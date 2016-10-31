using System;

namespace EntitySystem.Utility
{
	public static class ThrowIf
	{
		public static class Argument
		{
			public static T IsNull<T>(T param, string paramName)
			{
				if (param == null)
					throw new ArgumentNullException(paramName);
				return param;
			}
			
			public static void OutOfRange<T>(T value, T min, T max, string paramName,
			                                 bool minInclusive = true, bool maxInclusive = true)
				where T : IComparable<T>
			{
				ThrowIf.Argument.IsNull(value, paramName);
				if ((value.CompareTo(min) < (minInclusive ? 0 :  1)) ||
				    (value.CompareTo(max) > (maxInclusive ? 0 : -1)))
					throw new ArgumentOutOfRangeException(paramName, value,
						$"{ paramName } ({ value }) is out of range ({ min } - { max })");
			}
			
			public static void IsInvalidArrayIndex(Array array, int index,
				string arrayParamName = "array", string indexParamName = "index")
			{
				ThrowIf.Argument.IsNull(array, arrayParamName);
				if (array.Rank > 1) throw new RankException(
					$"{ arrayParamName } is a multi-dimensional array");
				if (index < array.GetLowerBound(0)) throw new ArgumentOutOfRangeException(indexParamName, index,
					$"{ indexParamName } ({ index }) is before the beginning of { arrayParamName }");
				if (index > array.GetUpperBound(0)) throw new ArgumentOutOfRangeException(indexParamName, index,
					$"{ indexParamName } ({ index }) after the end of { arrayParamName } ({ array.GetUpperBound(0) })");
			}
			
			public static void IsInvalidArrayRange(Array array, int index, int length,
				string arrayParamName = "array", string indexParamName = "index", string lengthParamName = "length")
			{
				ThrowIf.Argument.IsNull(array, arrayParamName);
				if (array.Rank > 1) throw new RankException(
					$"{ arrayParamName } is a multi-dimensional array");
				if (index < array.GetLowerBound(0)) throw new ArgumentOutOfRangeException(indexParamName, index,
					$"{ indexParamName } ({ index }) is before the beginning of { arrayParamName }");
				if (length < 0) throw new ArgumentOutOfRangeException(lengthParamName, length,
					$"{ lengthParamName } ({ length }) is negative");
				if (index + length - 1 > array.GetUpperBound(0)) throw new ArgumentException(
					$"{ indexParamName } + { lengthParamName } - 1 ({ index } + { length } - 1) is after the end of { arrayParamName } ({ array.GetUpperBound(0) })");
			}
		}
		
		public static class Params
		{
			public static T[] IsEmpty<T>(T[] array, string paramName)
			{
				ThrowIf.Argument.IsNull(array, paramName);
				if (array.Length == 0) throw new ArgumentException(
					$"{ paramName } is empty, at least one { nameof(T) } is required", paramName);
				return array;
			}
			
			public static T[] ContainsNull<T>(T[] array, string paramName)
			{
				ThrowIf.Argument.IsNull(array, paramName);
				foreach (var element in array)
					if (element == null) throw new ArgumentException(
						$"{ paramName } contains some null elements", paramName);
				return array;
			}
		}
	}
}
