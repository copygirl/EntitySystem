using System;

namespace EntitySystem.Components
{
	public struct Prototype : IComponent, IEquatable<Prototype>
	{
		public Entity Value { get; set; }
		
		public Prototype(Entity value) { Value = value; }
		
		
		public override bool Equals(object obj) =>
			((obj is Prototype) && Equals((Prototype)obj));
		public bool Equals(Prototype other) =>
			(Value == other.Value);
		
		public static bool operator ==(Prototype left, Prototype right) => left.Equals(right);
		
		public static bool operator !=(Prototype left, Prototype right) => !left.Equals(right);
		
		public override int GetHashCode() => Value.GetHashCode();
		
		
		public static implicit operator Entity(Prototype prototype) => prototype.Value;
		
		public static implicit operator Prototype(Entity value) => new Prototype(value);
	}
}
