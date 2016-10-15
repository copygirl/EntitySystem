using System.Collections;
using System.Collections.Generic;
using EntitySystem.Utility;
using EntitySystem.World;

namespace EntitySystem.Components.World
{
	using BlockEntityDictionary = EntitySystem.Collections.Dictionary<BlockPos, Entity>;
	
	public class ChunkBlockEntities : IComponent, IReadOnlyCollection<Entity>
	{
		readonly BlockEntityDictionary _entities = new BlockEntityDictionary();
		
		
		public Option<Entity> Get(BlockPos relative) => _entities[relative];
		
		public void Add(BlockPos relative, Entity entity) => _entities.Add(relative, entity);
		
		public Entity Remove(BlockPos relative) => _entities.Remove(relative);
		
		
		// IReadOnlyCollection implementation
		
		public int Count => _entities.Count;
		
		public IEnumerator<Entity> GetEnumerator() => _entities.Values.GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
