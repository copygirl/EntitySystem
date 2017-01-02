using System;

namespace EntitySystem.World
{
	public struct Facing : IEquatable<Facing>
	{
		public static Facing None { get; } = new Facing(0);
		
		public static Facing East { get; }  = new Facing(1); // +X
		public static Facing West { get; }  = new Facing(2); // -X
		public static Facing South { get; } = new Facing(3); // +Y
		public static Facing North { get; } = new Facing(4); // -Y
		public static Facing Up { get; }    = new Facing(5); // +Z
		public static Facing Down { get; }  = new Facing(6); // -Z
		
		static Facing[] _facingLookup = { None, East, West, South, North, Up, Down };
		static string[] _nameLookup = { nameof(None), nameof(East), nameof(West),
		                                nameof(South), nameof(North), nameof(Up), nameof(Down) };
		static int[,] _offsetTable = {
			{  0,  0,  0 },
			{  1,  0,  0 },
			{ -1,  0,  0 },
			{  0,  1,  0 },
			{  0, -1,  0 },
			{  0,  0,  1 },
			{  0,  0, -1 },
		};
		
		
		readonly byte _id;
		
		public int XOffset => _offsetTable[_id, 0];
		public int YOffset => _offsetTable[_id, 1];
		public int ZOffset => _offsetTable[_id, 2];
		
		public bool IsHorizontal => ((_id >= 1) && (_id <= 4));
		public bool IsVertical => (_id >= 5);
		
		public string Name => _nameLookup[_id];
		
		private Facing(byte id) { _id = id; }
		
		
		public byte ToByte() => _id;
		
		public static Facing FromByte(byte id) =>
			((id <= 6) ? _facingLookup[id] : None);
		
		public static Facing FromName(string name)
		{
			var index = Array.IndexOf(_nameLookup, name);
			return ((index >= 0) ? _facingLookup[index] : None);
		}
		
		
		public bool Equals(Facing other) => (_id == other._id);
		
		public override bool Equals(object obj) => ((obj is BlockPos) && Equals((BlockPos)obj));
		
		public override int GetHashCode() => _id.GetHashCode();
		
		public static bool operator ==(Facing left, Facing right) => left.Equals(right);
		
		public static bool operator !=(Facing left, Facing right) => !left.Equals(right);
		
		public override string ToString() => Name;
	}
}
