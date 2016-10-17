using System;
using EntitySystem.World;

namespace EntitySystem.Components.World
{
	public struct Chunk : IComponent, IEquatable<Chunk>
	{
		public const int SIZE = 16;
		public const int BITS = 4;
		public const int FLAG = ~(~0 << BITS);
		
		
		public ChunkPos Position { get; set; }
		
		public Chunk(ChunkPos position) { Position = position; }
		
		
		public override bool Equals(object obj) =>
			((obj is Chunk) && Equals((Chunk)obj));
		public bool Equals(Chunk other) =>
			(Position == other.Position);
		
		public static bool operator ==(Chunk left, Chunk right) => left.Equals(right);
		
		public static bool operator !=(Chunk left, Chunk right) => !left.Equals(right);
		
		public override int GetHashCode() => Position.GetHashCode();
		
		public override string ToString() => $"[{ nameof(Chunk) } { Position }]";
	}
}
