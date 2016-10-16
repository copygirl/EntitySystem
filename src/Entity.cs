using System;

namespace EntitySystem
{
	public struct Entity : IEquatable<Entity>, IComparable<Entity>
	{
		readonly uint _id;
		
		internal Entity(uint id) { _id = id; }
		
		
		public bool Equals(Entity other) => (_id == other._id);
		
		public int CompareTo(Entity other) => _id.CompareTo(other._id);
		
		public override bool Equals(object obj) => ((obj is Entity) && Equals((Entity)obj));
		
		public static bool operator ==(Entity left, Entity right) => left.Equals(right);
		
		public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
		
		public override int GetHashCode() => _id.GetHashCode();
		
		public override string ToString() => $"[Entity { _id :X}]";
	}
}
