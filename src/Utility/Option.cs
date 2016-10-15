using System;
using System.Collections.Generic;

namespace EntitySystem.Utility
{
	public struct Option<T> : IEquatable<Option<T>>
	{
		readonly T _value;
		
		public bool HasValue { get; private set; }
		
		public T Value => Expect(() =>
			new InvalidOperationException($"Option has no value"));
		
		
		public Option(T value, bool hasValue = true)
			{ _value = value; HasValue = hasValue; }
		
		public static Option<T> None { get; } = new Option<T>();
		public static Option<T> Some(T value) => new Option<T>(value);
		
		
		public T Expect(Func<Exception> func)
			{ if (HasValue) return _value; else throw func(); }
		
		public void ExpectNone(Func<Exception> func)
			{ if (!HasValue) throw func(); }
		
		
		public T Or(T @default) => (HasValue ? Value : @default);
		
		public T Or(Func<T> func) => (HasValue ? Value : func());
			
		public Option<T> Or(Func<Option<T>> func) => (HasValue ? this : func());
		
		public T OrDefault() => Or(default(T));
		
		
		public Option<TResult> Map<TResult>(Func<T, TResult> func) =>
			(HasValue ? Option<TResult>.Some(func(_value)) : Option<TResult>.None);
		
		public Option<TResult> Map<TResult>(Func<T, Option<TResult>> func) =>
			(HasValue ? func(_value) : Option<TResult>.None);
		
		public Option<TResult> Cast<TResult>() =>
			(HasValue ? (TResult)(object)_value : Option<TResult>.None);
		
		
		public static implicit operator Option<T>(T value)
		{
			if (value == null) throw new ArgumentNullException(
				"Implicit conversion to Option requires value to be non-null");
			return Option<T>.Some(value);
		}
		
		public static explicit operator T(Option<T> option) => option.Value;
		
		
		public override bool Equals(object obj) =>
			((obj is Option<T>) && Equals((Option<T>)obj));
		public bool Equals(Option<T> other) =>
			Equals(other, EqualityComparer<T>.Default);
		public bool Equals(Option<T> other, IEqualityComparer<T> comparer) =>
			((HasValue == other.HasValue) &&
			 (!HasValue || comparer.Equals(_value, other.Value)));
		
		public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
		
		public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
		
		public override int GetHashCode() =>
			(HasValue ? (_value?.GetHashCode() ?? 0) : 0);
		
		public override string ToString() =>
			(typeof(Option<T>).Name + "." + (HasValue ? "Some" : "None"));
	}
}
