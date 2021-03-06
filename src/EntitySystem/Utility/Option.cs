using System;
using System.Collections.Generic;

namespace EntitySystem.Utility
{
	public struct Option<T> : IEquatable<Option<T>>
	{
		readonly T _value;
		
		public bool HasValue { get; }
		
		public T Value => Expect(() =>
			new InvalidOperationException($"Option has no value"));
		
		
		public Option(T value, bool hasValue = true)
			{ _value = value; HasValue = hasValue; }
		
		public static Option<T> None { get; } = new Option<T>();
		public static Option<T> Some(T value) => new Option<T>(value);
		
		
		public T Expect(Func<Exception> func)
			{ if (HasValue) return _value; else throw func(); }
		
		public void ExpectNone(Func<Exception> func)
			{ if (HasValue) throw func(); }
		
		
		public T Or(T @default) => (HasValue ? _value : @default);
		
		public T Or(Func<T> func) => (HasValue ? _value : func());
		
		public Option<T> Or(Func<Option<T>> func) => (HasValue ? this : func());
		
		public T OrDefault() => Or(default(T));
		
		public bool TryGet(out T value) { value = OrDefault(); return HasValue; }
		
		
		public Option<TResult> Map<TResult>(Func<T, TResult> func) =>
			(HasValue ? Option<TResult>.Some(func(_value)) : Option<TResult>.None);
		
		public Option<TResult> Map<TResult>(Func<T, Option<TResult>> func) =>
			(HasValue ? func(_value) : Option<TResult>.None);
		
		public Option<TResult> Cast<TResult>() =>
			HasValue ? Option<TResult>.Some((TResult)(object)_value)
			         : Option<TResult>.None;
		
		
		public override bool Equals(object obj) =>
			((obj is Option<T>) && Equals((Option<T>)obj));
		
		public bool Equals(Option<T> other) =>
			Equals(other, EqualityComparer<T>.Default);
		public bool Equals(Option<T> other, IEqualityComparer<T> comparer) =>
			(other.HasValue ? Equals(other._value, comparer) : !HasValue);
		
		public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
		public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
		
		public bool Equals(T value) =>
			Equals(value, EqualityComparer<T>.Default);
		public bool Equals(T value, IEqualityComparer<T> comparer) =>
			(HasValue && comparer.Equals(_value, value));
		
		public static bool operator ==(Option<T> left, T right) => left.Equals(right);
		public static bool operator !=(Option<T> left, T right) => !left.Equals(right);
		public static bool operator ==(T left, Option<T> right) => right.Equals(left);
		public static bool operator !=(T left, Option<T> right) => !right.Equals(left);
		
		public override int GetHashCode() =>
			(HasValue ? (_value?.GetHashCode() ?? 0) : 0);
		
		public override string ToString() =>
			$"{ GetType().GetFriendlyName() }.{ Map((value) => $"Some( { value } )").Or("None") }";
	}
	
	public static class OptionExtensions
	{
		public static IEnumerable<T> WhereSome<T>(this IEnumerable<Option<T>> source)
		{
			foreach (var option in ThrowIf.Argument.IsNull(source, nameof(source)))
				if (option.HasValue)
					yield return option.Value;
		}
		
		public static IEnumerable<TResult> SelectSome<TSource, TResult>(
			this IEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
		{
			foreach (var value in ThrowIf.Argument.IsNull(source, nameof(source))) {
				var result = selector(value);
				if (result.HasValue)
					yield return result.Value;
			}
		}
	}
}
