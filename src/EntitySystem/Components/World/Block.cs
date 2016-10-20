using System;
using EntitySystem.World;

namespace EntitySystem.Components.World
{
	public struct Block : IComponent, IEquatable<Block>
	{
		public BlockPos Position { get; }
		
		public Block(BlockPos position) { Position = position; }
		
		
		public override bool Equals(object obj) =>
			((obj is Block) && Equals((Block)obj));
		public bool Equals(Block other) =>
			(Position == other.Position);
		
		public static bool operator ==(Block left, Block right) => left.Equals(right);
		
		public static bool operator !=(Block left, Block right) => !left.Equals(right);
		
		public override int GetHashCode() => Position.GetHashCode();
		
		public override string ToString() => $"[{ nameof(Block) } { Position }]";
	}
}
