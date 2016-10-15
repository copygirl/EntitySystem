using System;

namespace EntitySystem.World
{
	public class BlockStorage<T> : IComponent
		where T : struct, IComponent, IEquatable<T>
	{
		T[,,] _storage;
		
		/// <summary> Returns the number of non-default values in this BlockStorage.
		///           If this is 0, the storage array is in its default state. </summary>
		public int NonDefaultValues { get; private set; }
		
		public int SizeX { get { return _storage.GetLength(0); } }
		public int SizeY { get { return _storage.GetLength(1); } }
		public int SizeZ { get { return _storage.GetLength(2); } }
		
		public BlockStorage(int size) : this(size, size, size) {  }
		public BlockStorage(int sizeX, int sizeY, int sizeZ)
			: this(new T[sizeX, sizeY, sizeZ], 0) {  }
		public BlockStorage(T[,,] storage, int nonDefaultValues)
			{ _storage = storage; NonDefaultValues = nonDefaultValues; }
		
		
		public T Get(BlockPos pos) =>
			Get(pos.X, pos.Y, pos.Z);
		public T Get(int x, int y, int z) =>
			_storage[x, y, z];
		
		public T Set(BlockPos pos, T value) =>
			Set(pos.X, pos.Y, pos.Z, value);
		public T Set(int x, int y, int z, T value)
		{
			var previous = _storage[x, y, z];
			if (previous.Equals(default(T))) NonDefaultValues++;
			if (value.Equals(default(T))) NonDefaultValues--;
			_storage[x, y, z] = value;
			return previous;
		}
		
		
		public void Clear()
		{
			_storage = new T[SizeX, SizeY, SizeZ];
			NonDefaultValues = 0;
		}
	}
}
