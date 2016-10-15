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

		internal BlockRef(ChunkManager chunks, BlockPos pos)
		{
			Debug.Assert(chunks != null);
			ChunkManager = chunks;
			Position = pos;
		}
		
		// IEntityRef implementation
		
		// TODO: BlockStorage
		
		public Option<Entity> Entity => ChunkManager.GetChunkRef(Position)
			.Get<ChunkBlockEntities>().Map((blockEntities) =>
				blockEntities.Get(ChunkManager.GetChunkRelativeBlock(Position)));
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((block) => EntityManager[block].Components)
				.Or(Enumerable.Empty<IComponent>())
				// .Concat()
				.Follow(new Block(Position));
		
		public bool Has<T>() where T : IComponent =>
			Entity.Map((chunk) => EntityManager[chunk].Has<T>())
				.Or(false);
		
		public Option<T> Get<T>() where T : IComponent =>
			Entity.Map((block) => EntityManager[block].Get<T>());
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent =>
			EntityManager[ChunkManager.GetOrCreateChunk(Position)].Set<T>(value);
		
		public Option<T> Remove<T>() where T : IComponent =>
			Entity.Map((chunk) => EntityManager[chunk].Remove<T>());
	}
}
