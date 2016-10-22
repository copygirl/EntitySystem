using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class ChunkRef : IEntityRef
	{
		public ChunkManager ChunkManager { get; }
		public ChunkPos Position { get; }
		
		public EntityManager EntityManager => ChunkManager.EntityManager;
		
		internal ChunkRef(ChunkManager chunks, ChunkPos pos)
		{
			ThrowIf.Argument.IsNull(chunks, nameof(chunks));
			ChunkManager = chunks;
			Position = pos;
		}
		
		// IEntityRef implementation
		
		public Option<Entity> Entity => ChunkManager.GetChunkEntity(Position);
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((chunk) => EntityManager[chunk].Components)
				.Or(() => new IComponent[]{ new Chunk(Position) }.AsEnumerable());
		
		public bool Has<T>() where T : IComponent =>
			(typeof(T) == typeof(Chunk)) ||
				Entity.Map((chunk) => EntityManager[chunk].Has<T>()).Or(false);
		
		public Option<T> Get<T>() where T : IComponent =>
			(typeof(T) == typeof(Chunk)) ? (T)(object)new Chunk(Position) :
				Entity.Map((chunk) => EntityManager[chunk].Get<T>());
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			if (typeof(T) == typeof(Chunk)) throw new InvalidOperationException(
				$"{ nameof(Chunk) } cannot be modified");
			return EntityManager[ChunkManager.GetOrCreateChunkEntity(Position)].Set<T>(value);
		}
		
		public Option<T> Remove<T>() where T : IComponent
		{
			if (typeof(T) == typeof(Chunk)) throw new InvalidOperationException(
				$"{ nameof(Chunk) } cannot be modified");
			return Entity.Map((chunk) => EntityManager[chunk].Remove<T>());
		}
		
		// ToString
		
		public override string ToString() => $"[{ nameof(ChunkRef) } { Position }]";
	}
}
