using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class BlockRef : IEntityRef
	{
		public ChunkManager ChunkManager { get; private set; }
		public BlockPos Position { get; private set; }
		
		public EntityManager EntityManager => ChunkManager.EntityManager;
		public ChunkRef Chunk => ChunkManager.GetChunkRef(Position);
		public BlockPos ChunkRelPos => ChunkManager.GetChunkRelPos(Position);
		
		internal BlockRef(ChunkManager chunks, BlockPos pos)
		{
			Debug.Assert(chunks != null);
			ChunkManager = chunks;
			Position = pos;
		}
		
		
		IEnumerable<IComponent> GetBlockStorageComponents() =>
			Chunk.Components
				.Select((component) =>
					(IsChunkBlockStorage(component)
						? (IComponent)((dynamic)component)
							.GetNonDefault(ChunkRelPos)
							.Cast<IComponent>().Or(null)
						: null))
				.Where((component) => (component != null));
		
		static bool IsChunkBlockStorage(IComponent component)
		{
			var typeInfo = component.GetType().GetTypeInfo();
			return (typeInfo.IsGenericType && (typeInfo.GetGenericTypeDefinition() == typeof(ChunkBlockStorage<>)));
		}
		
		
		// IEntityRef implementation
		
		public Option<Entity> Entity =>
			Chunk.Get<ChunkBlockEntities>().Map((blockEntities) =>
				blockEntities.Get(ChunkRelPos));
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((block) => EntityManager[block].Components)
				.Or(Enumerable.Empty<IComponent>())
				.Concat(GetBlockStorageComponents())
				.Follow(new Block(Position));
		
		public bool Has<T>() where T : IComponent =>
			Entity.Map((block) => EntityManager[block].Has<T>())
				.Or(() => Chunk.Has<ChunkBlockStorage<T>>());
		
		public Option<T> Get<T>() where T : IComponent =>
			(typeof(T) == typeof(Block)) ? (T)(object)new Block(Position) :
				Entity.Map((block) => EntityManager[block].Get<T>())
					.Or(() => Chunk.Get<ChunkBlockStorage<T>>()
						.Map((storage) => storage.Get(ChunkRelPos)));
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			if (typeof(T) == typeof(Block)) throw new InvalidOperationException(
				$"{ nameof(Block) } cannot be modified");
			throw new NotImplementedException();
		}
		
		public Option<T> Remove<T>() where T : IComponent
		{
			if (typeof(T) == typeof(Block)) throw new InvalidOperationException(
				$"{ nameof(Block) } cannot be modified");
			return Entity.Map((block) => EntityManager[block].Remove<T>())
				.Or(() => Chunk.Get<ChunkBlockStorage<T>>()
					.Map((storage) => storage.Remove(ChunkRelPos)));
		}
		
		
		// ToString
		
		public override string ToString() => $"[{ nameof(BlockRef) } { Position }]";
	}
}
