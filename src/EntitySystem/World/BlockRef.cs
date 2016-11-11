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
		
		public Option<T> Set<T>(Option<T> valueOption) where T : IComponent
		{
			if (typeof(T) == typeof(Block)) throw new InvalidOperationException(
				$"{ nameof(Block) } cannot be modified");
			
			T value;
			var hasValue = valueOption.TryGet(out value);
			
			ChunkManager.StorageHandler<T> handler;
			return Chunks.Storage.Get<T>().TryGet(out handler)
				// If a storage handler was registered for this component type,
				// the value will be stored in a chunk's block storage component.
				? (hasValue && !EqualityComparer<T>.Default.Equals(default(T), value))
					// Block storage stores unset values as defaults, hence the additional check.
					// If the value is being set (and not default), create a new chunk and block storage if needed.
					? handler
						.GetOrAdd(Chunks.GetOrCreateChunkEntity(Position))
						.Set(ChunkRelPos, value)
					// If the value is being unset, remove (internally, set to default)
					// the component, if the chunk and block storage component exist.
					: Chunks.GetChunkEntity(Position)
						.Map((chunk) => handler.Get(chunk))
						.Map((storage) => storage.Remove(ChunkRelPos))
				// No storage handler means components are stored on a block entity.
				: hasValue
					? Entities[GetOrCreateEntity()].Set(valueOption)
					: Entity.Map((block) => Entities[block].Remove<T>());
		}
		
		
		// ToString
		
		public override string ToString() => $"[{ nameof(BlockRef) } { Position }]";
	}
}
