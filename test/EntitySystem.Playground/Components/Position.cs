using System;
using EntitySystem.Utility;

namespace EntitySystem.Playground.Components
{
	public struct Position : IComponent, IEquatable<Position>
	{
		public int X { get; }
		public int Y { get; }
		
		public Position(int x, int y) { X = x; Y = y; }
		
		public Position Add(int x, int y) => new Position(X + x, Y + y);
		
		public Position Left  => Add(-1,  0);
		public Position Right => Add( 1,  0);
		public Position Up    => Add( 0, -1);
		public Position Down  => Add( 0,  1);
		
		public static Position operator +(Position left, Position right) => left.Add( right.X,  right.Y);
		public static Position operator -(Position left, Position right) => left.Add(-right.X, -right.Y);
		
		public bool Equals(Position other) => ((other.X == X) && (other.Y == Y));
		public override bool Equals(object other) => ((other is Position) && Equals((Position)other));
		public override int GetHashCode() => HashHelper.For(X, Y);
		
		public override string ToString() => $"[{ nameof(Position) } { X }:{ Y }]";
	}
}
