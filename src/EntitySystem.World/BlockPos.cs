using System;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public struct BlockPos : IEquatable<BlockPos>
	{
		public static readonly BlockPos ORIGIN = new BlockPos(0, 0, 0);
		
		
		public int X { get; }
		public int Y { get; }
		public int Z { get; }
		
		public BlockPos(int x, int y, int z) { X = x; Y = y; Z = z; }
		
		
		public BlockPos Offset(BlockPos pos) => Offset(pos.X, pos.Y, pos.Z);
		public BlockPos Offset(int x, int y, int z) => new BlockPos(X + x, Y + y, Z + z);
		
		public BlockPos Offset(Facing facing, int distance = 1) =>
			Offset(facing.XOffset * distance, facing.YOffset * distance, facing.ZOffset * distance);
		
		
		public static BlockPos operator -(BlockPos pos) => new BlockPos(-pos.X, -pos.Y, -pos.Z);
		
		public static BlockPos operator +(BlockPos left, BlockPos right) => left.Offset(right);
		
		public static BlockPos operator -(BlockPos left, BlockPos right) => left.Offset(-right.X, -right.Y, -right.Z);
		
		
		public bool Equals(BlockPos other) =>
			((X == other.X) && (Y == other.Y) && (Z == other.Z));
		
		public override bool Equals(object obj) =>
			((obj is BlockPos) && Equals((BlockPos)obj));
		
		public static bool operator ==(BlockPos left, BlockPos right) => left.Equals(right);
		
		public static bool operator !=(BlockPos left, BlockPos right) => !left.Equals(right);
		
		public override int GetHashCode() => HashHelper.For(X, Y, Z);
		
		public override string ToString() => $"{ X }:{ Y }:{ Z }";
	}
}
