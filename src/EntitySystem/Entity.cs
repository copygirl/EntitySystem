using System;

namespace EntitySystem
{
	public struct Entity : IEquatable<Entity>, IComparable<Entity>
	{
		public static readonly Entity None = default(Entity);
		
		
		public uint ID { get; }
		
		public bool IsSome => (ID != default(uint));
		public bool IsNone => (ID == default(uint));
		
		internal Entity(uint id) { ID = id; }
		
		
		public bool Equals(Entity other) => (ID == other.ID);
		
		public int CompareTo(Entity other) => ID.CompareTo(other.ID);
		
		public override bool Equals(object obj) => ((obj is Entity) && Equals((Entity)obj));
		
		public static bool operator ==(Entity left, Entity right) => left.Equals(right);
		
		public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
		
		public override int GetHashCode() => ID.GetHashCode();
		
		public override string ToString() =>
			IsSome ? $"[{ nameof(Entity) } { ID }]"
			       : $"[{ nameof(Entity) } None]";
	}
}
