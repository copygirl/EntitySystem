using System;
using System.Collections.Generic;
using System.Reflection;
using EntitySystem.Utility;
using EntitySystem.World;

namespace EntitySystem.Components.World
{
	public class ChunkBlockStorage<T> : IComponent
		where T : IComponent
	{
		const int STORAGE_SIZE = Chunk.SIZE * Chunk.SIZE * Chunk.SIZE;
		
		static readonly IEqualityComparer<T> COMPARER = EqualityComparer<T>.Default;
		
		
		T[] _storage = new T[STORAGE_SIZE];
		
		/// <summary> Returns the number of non-default values in this ChunkBlockStorage.
		///           If this is 0, the storage array is in its default state. </summary>
		public int NonDefaultValues { get; private set; }
		
		public ChunkBlockStorage()
		{
			if (!typeof(T).GetTypeInfo().IsValueType)
				throw new InvalidOperationException(
					$"{ typeof(T) } is not a value type");
		}
		
		
		public T Get(BlockPos relative) =>
			Get(relative.X, relative.Y, relative.Z);
		public T Get(int x, int y, int z) =>
			_storage[GetIndex(x, y, z)];
		
		public T Set(BlockPos relative, T value) =>
			Set(relative.X, relative.Y, relative.Z, value);
		public T Set(int x, int y, int z, T value) {
			var index = GetIndex(x, y, z);
			var previous = _storage[index];
			if (!COMPARER.Equals(default(T), previous)) NonDefaultValues--;
			if (!COMPARER.Equals(default(T), value)) NonDefaultValues++;
			_storage[index] = value;
			return previous;
		}
		
		
		int GetIndex(int x, int y, int z) {
			#if DEBUG
				ThrowIf.Argument.OutOfRange(x, 0, Chunk.SIZE, nameof(x), maxInclusive: false);
				ThrowIf.Argument.OutOfRange(y, 0, Chunk.SIZE, nameof(y), maxInclusive: false);
				ThrowIf.Argument.OutOfRange(z, 0, Chunk.SIZE, nameof(z), maxInclusive: false);
			#endif
			return (x | y << Chunk.BITS | z << (Chunk.BITS * 2));
		}
	}
}
