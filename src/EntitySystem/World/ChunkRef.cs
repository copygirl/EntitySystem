using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Components.World;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class ChunkRef : IEntityRef
	{
		public ChunkManager Chunks { get; }
		public ChunkPos Position { get; }
		
		public EntityManager Entities => Chunks.Entities;
		
		internal ChunkRef(ChunkManager chunks, ChunkPos pos)
		{
			ThrowIf.Argument.IsNull(chunks, nameof(chunks));
			Chunks   = chunks;
			Position = pos;
		}
		
		// IEntityRef implementation
		
		public Option<Entity> Entity => Chunks.GetChunkEntity(Position);
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((chunk) => Entities[chunk].Components)
				.Or(() => new IComponent[]{ new Chunk(Position) }.AsEnumerable());
		
		public Option<T> Get<T>() where T : IComponent =>
			(typeof(T) == typeof(Chunk)) ? (T)(object)new Chunk(Position) :
				Entity.Map((chunk) => Entities[chunk].Get<T>());
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			if (typeof(T) == typeof(Chunk)) throw new InvalidOperationException(
				$"{ nameof(Chunk) } cannot be modified");
			return ((value.HasValue)
				? Entities[Chunks.GetOrCreateChunkEntity(Position)].Set<T>(value)
				: Entity.Map((chunk) => Entities[chunk].Set<T>(value)));
		}
		
		// ToString
		
		public override string ToString() => $"[{ nameof(ChunkRef) } { Position }]";
	}
}
