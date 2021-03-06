using System;
using EntitySystem.Collections;
using EntitySystem.Utility;
using EntitySystem.World.Components;

namespace EntitySystem.World
{
	public class ChunkManager
	{
		public EntityManager Entities { get; }
		public StorageRegistry Storage { get; }
		
		public ChunkManager(EntityManager entities)
		{
			Entities = ThrowIf.Argument.IsNull(entities, nameof(entities));
			Storage  = new StorageRegistry(this);
		}
		
		
		// Accessing blocks / chunks
		
		readonly OptionDictionary<ChunkPos, Entity> _chunks =
			new OptionDictionary<ChunkPos, Entity>();
		
		
		public BlockRef Get(BlockPos pos) => new BlockRef(this, pos);
		public ChunkRef GetChunk(BlockPos pos) => GetChunk(GetChunkPos(pos));
		public ChunkRef GetChunk(ChunkPos pos) => new ChunkRef(this, pos);
		
		public Option<Entity> GetChunkEntity(BlockPos pos) => GetChunkEntity(GetChunkPos(pos));
		public Option<Entity> GetChunkEntity(ChunkPos pos) => _chunks[pos];
		
		public Entity GetOrCreateChunkEntity(BlockPos pos) =>
			GetOrCreateChunkEntity(GetChunkPos(pos));
		public Entity GetOrCreateChunkEntity(ChunkPos pos) =>
			_chunks.GetOrAdd(pos, (_) => Entities.New(new Chunk(pos)));
		
		
		public ChunkPos GetChunkPos(BlockPos pos) =>
			new ChunkPos((pos.X >> Chunk.BITS), (pos.Y >> Chunk.BITS), (pos.Z >> Chunk.BITS));
		
		public BlockPos GetChunkRelPos(BlockPos pos) =>
			new BlockPos((pos.X & Chunk.FLAG), (pos.Y & Chunk.FLAG), (pos.Z & Chunk.FLAG));
		
		
		// Storage registry
		
		public class StorageRegistry
		{
			readonly ChunkManager _chunks;
			readonly OptionDictionary<Type, IStorageHandler> _storageRegistry =
				new OptionDictionary<Type, IStorageHandler>();
			
			internal StorageRegistry(ChunkManager chunks) { _chunks = chunks; }
			
			
			public void Register<T>() where T : struct, IComponent =>
				_storageRegistry.Add(typeof(T),
					new StorageHandler<T>(typeof(T), _chunks.Entities));
			
			public Option<StorageHandler<T>> Get<T>() where T : IComponent =>
				Get(typeof(T)).Cast<StorageHandler<T>>();
			public Option<IStorageHandler> Get(Type componentType) =>
				_storageRegistry[componentType];
		}
		
		public interface IStorageHandler
		{
			Type ComponentType { get; }
		}
		
		public class StorageHandler<T> : IStorageHandler where T : IComponent
		{
			readonly EntityManager _entities;
			public Type ComponentType { get; private set; }
			
			internal StorageHandler(Type componentType, EntityManager entityManager)
			{
				ComponentType = componentType;
				_entities = entityManager;
			}
			
			
			public Option<ChunkBlockStorage<T>> Get(Entity chunk) =>
				_entities[chunk].Get<ChunkBlockStorage<T>>();
				
			public ChunkBlockStorage<T> GetOrAdd(Entity chunk) =>
				_entities[chunk].GetOrAdd(() => new ChunkBlockStorage<T>());
		}
	}
}
