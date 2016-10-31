using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class EntityManager : IEnumerable<EntityRef>
	{
		readonly HashSet<Entity> _entities = new HashSet<Entity>();
		uint _entityIdCounter = 1;
		
		
		public ComponentManager Components { get; }
		
		public EntityRef this[Entity entity] => new EntityRef(this, entity);
		
		
		public event Action<Entity> Added;
		public event Action<Entity> Removed;
		
		
		public EntityManager()
		{
			Components = new ComponentManager(this);
		}
		
		
		public bool Has(Entity entity) =>
			_entities.Contains(entity);
		
		public EntityRef New()
		{
			Entity entity;
			do { entity = new Entity(unchecked(_entityIdCounter++)); }
			while (!_entities.Add(entity));
			
			Added?.Invoke(entity);
			return new EntityRef(this, entity);
		}
		
		public EntityRef New(params IComponent[] components)
		{
			ThrowIf.Params.ContainsNull(components, nameof(components));
			var entity = New();
			// Add all components dynamically using reflection (slow)
			var method = typeof(EntityRefExtensions).GetTypeInfo()
				.GetMethod(nameof(EntityRefExtensions.Set));
			foreach (var component in components)
				method.MakeGenericMethod(component.GetType())
					.Invoke(null, new object[]{ entity, component });
			return entity;
		}
		
		public void Remove(Entity entity) {
			if (!_entities.Remove(entity))
				throw new EntityNonExistantException(this, entity);
			Removed?.Invoke(entity);
		}
		
		
		// IEnumerable implementation
		
		public IEnumerator<EntityRef> GetEnumerator() =>
			_entities.Select((entity) => this[entity]).GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
