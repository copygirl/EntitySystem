using System;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public struct ChunkPos : IEquatable<ChunkPos>
	{
		public int X { get; }
		public int Y { get; }
		public int Z { get; }
		
		public ChunkPos(int x, int y, int z) { X = x; Y = y; Z = z; }
		
		
		public bool Equals(ChunkPos other) =>
			((X == other.X) && (Y == other.Y) && (Z == other.Z));
		
		public override bool Equals(object obj) =>
			((obj is ChunkPos) && Equals((ChunkPos)obj));
		
		public static bool operator ==(ChunkPos left, ChunkPos right) => left.Equals(right);
		
		public static bool operator !=(ChunkPos left, ChunkPos right) => !left.Equals(right);
		
		public override int GetHashCode() => HashHelper.For(X, Y, Z);
		
		public override string ToString() => $"{ X }:{ Y }:{ Z }";
		
	}
}
