using System.Collections.Generic;
using System.Linq;
using EntitySystem.Utility;

namespace EntitySystem.World
{
	public class ChunkRef : IEntityRef
	{
		public ChunkManager ChunkManager { get; private set; }
		public ChunkPos Position { get; private set; }
		
		public EntityManager EntityManager => ChunkManager.EntityManager;
		
		internal ChunkRef(ChunkManager chunks, ChunkPos pos)
		{
			ThrowIf.Argument.IsNull(chunks, nameof(chunks));
			ChunkManager = chunks;
			Position = pos;
		}
		
		// IEntityRef implementation
		
		public Option<Entity> Entity => ChunkManager.GetChunk(Position);
		
		public IEnumerable<IComponent> Components =>
			Entity.Map((chunk) => EntityManager[chunk].Components)
				.Or(Enumerable.Empty<IComponent>());
		
		public bool Has<T>() where T : IComponent =>
			Entity.Map((chunk) => EntityManager[chunk].Has<T>()).Or(false);
		
		public Option<T> Get<T>() where T : IComponent =>
			Entity.Map((chunk) => EntityManager[chunk].Get<T>());
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent =>
			EntityManager[ChunkManager.GetOrCreateChunk(Position)].Set<T>(value);
		
		public Option<T> Remove<T>() where T : IComponent =>
			Entity.Map((chunk) => EntityManager[chunk].Remove<T>());
		
		// ToString
		
		public override string ToString() => $"[{ nameof(ChunkRef) } { Position }]";
	}
}
