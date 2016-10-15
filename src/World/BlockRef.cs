using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
		
		// IEntityRef implementation
		
		public Option<Entity> Entity =>
			Chunk.Get<ChunkBlockEntities>().Map((blockEntities) =>
				blockEntities.Get(ChunkRelPos));
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((block) => EntityManager[block].Components)
				.Or(Enumerable.Empty<IComponent>())
				// .Concat() with ChunkBlockStorage values
				.Follow(new Block(Position));
		
		public bool Has<T>() where T : IComponent =>
			Entity.Map((block) => EntityManager[block].Has<T>())
				.Or(() => Chunk.Has<ChunkBlockStorage<T>>());
		
		public Option<T> Get<T>() where T : IComponent =>
			Entity.Map((block) => EntityManager[block].Get<T>())
				.Or(() => Chunk.Get<ChunkBlockStorage<T>>()
					.Map((storage) => storage.Get(ChunkRelPos)));
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			throw new NotImplementedException();
		}
		
		public Option<T> Remove<T>() where T : IComponent =>
			Entity.Map((block) => EntityManager[block].Remove<T>())
				.Or(() => Chunk.Get<ChunkBlockStorage<T>>()
					.Map((storage) => storage.Set(ChunkRelPos, default(T))));
	}
}
