using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class BlockRef : IEntityRef
	{
		public ChunkManager ChunkManager { get; }
		public BlockPos Position { get; }
		
		public EntityManager EntityManager => ChunkManager.EntityManager;
		public ChunkRef Chunk => ChunkManager.GetChunk(Position);
		public BlockPos ChunkRelPos => ChunkManager.GetChunkRelPos(Position);
		
		internal BlockRef(ChunkManager chunkManager, BlockPos pos)
		{
			ChunkManager = ThrowIf.Argument.IsNull(chunkManager, nameof(chunkManager));;
			Position     = pos;
		}
		
		
		IEnumerable<IComponent> GetBlockStorageComponents() =>
			Chunk.Components
				.Select((component) => (component as IChunkBlockStorage)?.Get(ChunkRelPos).OrDefault())
				.Where((component) => (component != null));
		
		Entity GetOrCreateEntity() =>
			Chunk.GetOrAdd(() => new ChunkBlockEntities())
				.GetOrAdd(ChunkRelPos, () => EntityManager.New(new Block(Position)));
		
		
		// IEntityRef implementation
		
		public Option<Entity> Entity =>
			Chunk.Get<ChunkBlockEntities>().Map((entities) =>
				entities.Get(ChunkRelPos));
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((block) => EntityManager[block].Components)
				.Or(() => new IComponent[]{ new Block(Position) }.AsEnumerable())
				.Concat(GetBlockStorageComponents());
		
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
			var handler = ChunkManager.Storage.Get<T>();
			return (handler.HasValue
				? (value.HasValue
					? handler.Value
						.GetOrAdd(ChunkManager.GetOrCreateChunkEntity(Position))
						.Set(ChunkRelPos, value)
					: ChunkManager.GetChunkEntity(Position)
						.Map((chunk) => handler.Value.Get(chunk))
						.Map((storage) => storage.Remove(ChunkRelPos)))
				: (value.HasValue
					? EntityManager[GetOrCreateEntity()].Set(value)
					: Entity.Map((block) => EntityManager[block].Set(value))));
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
