using System;
using System.Collections.Generic;

namespace EntitySystem
{
	public class EntityManager
	{
		public IEnumerable<Entity> Entities { get {
			throw new NotImplementedException();
		} }
		
		public IEntityRef this[Entity entity] { get {
			throw new NotImplementedException();
		} }
		
		public Entity New(params IComponent[] components)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(Entity entity)
		{
			throw new NotImplementedException();
		}
	}
}
