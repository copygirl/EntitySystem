using System;
using System.Collections.Generic;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class ChunkManager
	{
		public const int CHUNK_SIZE = 16;
		public const int CHUNK_BITS = 4;
		public const int CHUNK_FLAG = ~(~0 << CHUNK_BITS);
		
		
		readonly Dictionary<ChunkPos, Entity> _chunks =
			new Dictionary<ChunkPos, Entity>();
			
		public EntityManager EntityManager { get; private set; }
		
		public ChunkManager(EntityManager entityManager)
		{
			EntityManager = ThrowIf.Argument.IsNull(entityManager, nameof(entityManager));
		}
		
		
		public BlockRef Get(BlockPos pos) => new BlockRef(this, pos);
		public ChunkRef GetChunkRef(BlockPos pos) => GetChunkRef(GetChunkFromBlock(pos));
		public ChunkRef GetChunkRef(ChunkPos pos) => new ChunkRef(this, pos);
		
		public Option<Entity> GetChunk(BlockPos pos) => GetChunk(GetChunkFromBlock(pos));
		public Option<Entity> GetChunk(ChunkPos pos) => _chunks[pos];
		
		public Entity GetOrCreateChunk(BlockPos pos) => GetOrCreateChunk(GetChunkFromBlock(pos));
		public Entity GetOrCreateChunk(ChunkPos pos) => _chunks.GetOrAdd(pos, (_) =>
			EntityManager.New(new Chunk(pos)));
		
		
		public ChunkPos GetChunkFromBlock(BlockPos pos) =>
			new ChunkPos((pos.X >> CHUNK_BITS), (pos.Y >> CHUNK_BITS), (pos.Z >> CHUNK_BITS));
		
		public BlockPos GetChunkRelativeBlock(BlockPos pos) =>
			new BlockPos((pos.X & CHUNK_FLAG), (pos.Y & CHUNK_FLAG), (pos.Z & CHUNK_FLAG));
	}
}
