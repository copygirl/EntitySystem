using System;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public struct BlockRegion : IEquatable<BlockRegion>
	{
		public static readonly BlockRegion ZERO = new BlockRegion(0, 0, 0, 0, 0, 0);
		public static readonly BlockRegion MAX  = new BlockRegion(
			int.MinValue / 2, int.MinValue / 2, int.MinValue / 2,
			int.MaxValue / 2, int.MaxValue / 2, int.MaxValue / 2);
		
		
		public int MinX { get; }
		public int MinY { get; }
		public int MinZ { get; }
		
		public int MaxX { get; }
		public int MaxY { get; }
		public int MaxZ { get; }
		
		public int SizeX => MaxX - MinX;
		public int SizeY => MaxY - MinY;
		public int SizeZ => MaxZ - MinZ;
		
		public BlockPos Min => new BlockPos(MinX, MinY, MinZ);
		public BlockPos Max => new BlockPos(MaxX, MaxY, MaxZ);
		
		public BlockRegion(int minX, int minY, int minZ,
		                   int maxX, int maxY, int maxZ)
		{
			if ((maxX < minX) || (maxY < minY) || (maxZ < minZ)) throw new ArgumentException(
				$"maxXYZ ({ maxX }, { maxY }, { maxZ }) is smaller than minXYZ ({ minX }, { minY }, { minZ })");
			
			MinX = minX; MinY = minY; MinZ = minZ;
			MaxX = maxX; MaxY = maxY; MaxZ = maxZ;
		}
		
		
		public static BlockRegion FromEnclosing(params BlockPos[] positions)
		{
			ThrowIf.Params.IsEmpty(positions, nameof(positions));
			
			int minX, minY, minZ, maxX, maxY, maxZ;
			minX = minY = minZ = int.MaxValue;
			maxX = maxY = maxZ = int.MinValue;
			
			foreach (var pos in positions) {
				if (pos.X < minX) minX = pos.X;
				if (pos.Y < minY) minY = pos.Y;
				if (pos.Z < minZ) minZ = pos.Z;
				if (pos.X >= maxX) maxX = pos.X + 1;
				if (pos.Y >= maxY) maxY = pos.Y + 1;
				if (pos.Z >= maxZ) maxZ = pos.Z + 1;
			}
			
			return new BlockRegion(minX, minY, minZ, maxX, maxY, maxZ);
		}
		
		public static BlockRegion FromCenter(BlockPos center, int radius) =>
			BlockRegion.FromCenter(center, radius, radius, radius);
		public static BlockRegion FromCenter(BlockPos center, int radiusX, int radiusY, int radiusZ) =>
			new BlockRegion(center.X - radiusX, center.Y - radiusY, center.Z - radiusZ,
			                center.X + radiusX + 1, center.Y + radiusY + 1, center.Z + radiusZ + 1);
		
		
		public bool Contains(BlockPos pos) => Contains(pos.X, pos.Y, pos.Z);
		public bool Contains(int x, int y, int z) =>
			((x >= MinX) && (y >= MinY) && (z >= MinZ) &&
			 (x < MaxX) && (y < MaxY) && (z < MaxZ));
		public bool Contains(BlockRegion region) =>
			((region.MinX >= MinX) && (region.MinY >= MinY) && (region.MinZ >= MinZ) &&
			 (region.MaxX <= MaxX) && (region.MaxY <= MaxY) && (region.MaxZ <= MaxZ));
		
		public bool Intersects(BlockRegion region) =>
			(((region.MaxX > MinX) || (region.MinX < MaxX)) &&
			 ((region.MaxY > MinY) || (region.MinY < MaxY)) &&
			 ((region.MaxZ > MinZ) || (region.MinZ < MaxZ)));
		
			
		public BlockRegion Offset(int x, int y, int z) =>
			new BlockRegion(MinX + x, MinY + y, MinZ + z, MaxX + x, MaxY + y, MaxZ + z);
		public BlockRegion Offset(BlockPos pos) => Offset(pos.X, pos.Y, pos.Z);
			
		public BlockRegion Offset(Facing facing, int distance = 1) =>
			Offset(facing.XOffset * distance, facing.YOffset * distance, facing.ZOffset * distance);
		
		
		public BlockRegion Expand(int amount) =>
			Expand(amount, amount, amount, amount, amount, amount);
		public BlockRegion Expand(int amountX, int amountY, int amountZ) =>
			Expand(amountX, amountY, amountZ, amountX, amountY, amountZ);
		public BlockRegion Expand(int minX, int minY, int minZ,
		                          int maxX, int maxY, int maxZ) =>
			new BlockRegion(MinX - minX, MinY - minY, MinZ - minZ,
			                MaxX + maxX, MaxY + maxY, MaxZ + maxZ);
		
		
		public static BlockRegion operator +(BlockRegion region, BlockPos pos) => region.Offset(pos);
		
		public static BlockRegion operator -(BlockRegion region, BlockPos pos) => region.Offset(-pos);
		
		
		public bool Equals(BlockRegion other) =>
			((MinX == other.MinX) && (MinY == other.MinY) && (MinZ == other.MinZ) &&
			 (MaxX == other.MaxX) && (MaxY == other.MaxY) && (MaxZ == other.MaxZ));
		
		public override bool Equals(object obj) =>
			((obj is BlockRegion) && Equals((BlockRegion)obj));
		
		public static bool operator ==(BlockRegion left, BlockRegion right) => left.Equals(right);
		
		public static bool operator !=(BlockRegion left, BlockRegion right) => !left.Equals(right);
		
		public override int GetHashCode() => HashHelper.For(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
		
		public override string ToString() => $"{ MinX }:{ MinY }:{ MinZ } - { MaxX }:{ MaxY }:{ MaxZ }";
	}
}
