using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class BlockRef : IEntityRef
	{
		public ChunkManager Chunks { get; }
		public BlockPos Position { get; }
		
		public EntityManager Entities => Chunks.Entities;
		public ChunkRef Chunk => Chunks.GetChunk(Position);
		public BlockPos ChunkRelPos => Chunks.GetChunkRelPos(Position);
		
		internal BlockRef(ChunkManager chunks, BlockPos pos)
		{
			Chunks   = ThrowIf.Argument.IsNull(chunks, nameof(chunks));;
			Position = pos;
		}
		
		
		IEnumerable<IComponent> GetBlockStorageComponents() =>
			Chunk.Components
				.Select((component) => (component as IChunkBlockStorage)?.Get(ChunkRelPos).OrDefault())
				.Where((component) => (component != null));
		
		Entity GetOrCreateEntity() =>
			Chunk.GetOrAdd(() => new ChunkBlockEntities())
				.GetOrAdd(ChunkRelPos, () => Entities.New(new Block(Position)));
		
		
		// IEntityRef implementation
		
		public Option<Entity> Entity =>
			Chunk.Get<ChunkBlockEntities>().Map((entities) =>
				entities.Get(ChunkRelPos));
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((block) => Entities[block].Components)
				.Or(() => new IComponent[]{ new Block(Position) }.AsEnumerable())
				.Concat(GetBlockStorageComponents());
		
		public Option<T> Get<T>() where T : IComponent =>
			(typeof(T) == typeof(Block)) ? (T)(object)new Block(Position) :
				Entity.Map((block) => Entities[block].Get<T>())
					.Or(() => Chunk.Get<ChunkBlockStorage<T>>()
						.Map((storage) => storage.Get(ChunkRelPos)));
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			if (typeof(T) == typeof(Block)) throw new InvalidOperationException(
				$"{ nameof(Block) } cannot be modified");
			var handler = Chunks.Storage.Get<T>();
			return (handler.HasValue
				? (value.HasValue
					? handler.Value
						.GetOrAdd(Chunks.GetOrCreateChunkEntity(Position))
						.Set(ChunkRelPos, value)
					: Chunks.GetChunkEntity(Position)
						.Map((chunk) => handler.Value.Get(chunk))
						.Map((storage) => storage.Remove(ChunkRelPos)))
				: (value.HasValue
					? Entities[GetOrCreateEntity()].Set(value)
					: Entity.Map((block) => Entities[block].Set(value))));
		}
		
		
		// ToString
		
		public override string ToString() => $"[{ nameof(BlockRef) } { Position }]";
	}
}
