using System;
using System.Collections;
using System.Collections.Generic;
using EntitySystem.Collections;
using EntitySystem.Utility;
using EntitySystem.World;

namespace EntitySystem.Components.World
{
	public class ChunkBlockEntities : IComponent, IReadOnlyCollection<Entity>
	{
		readonly OptionDictionary<BlockPos, Entity> _entities = new OptionDictionary<BlockPos, Entity>();
		
		
		public Option<Entity> Get(BlockPos relative) => _entities[relative];
		
		public Entity Add(BlockPos relative, Entity entity)
			{ _entities.Add(relative, entity); return entity; }
		
		public Entity GetOrAdd(BlockPos relative, Func<Entity> entityFactory) =>
			_entities.GetOrAdd(relative, (_) => entityFactory());
		
		public Entity Remove(BlockPos relative) => _entities.Remove(relative);
		
		
		// IReadOnlyCollection implementation
		
		public int Count => _entities.Count;
		
		public IEnumerator<Entity> GetEnumerator() => _entities.Values.GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
