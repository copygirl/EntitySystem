using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitySystem.Collections;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class EntityManager : IEnumerable<EntityRef>
	{
		internal readonly OptionDictionary<Entity, TypedCollection<IComponent>> _entities =
			new OptionDictionary<Entity, TypedCollection<IComponent>>();
		
		uint _entityIdCounter = 1;
		
		
		public IEnumerable<Entity> Entities => _entities.Keys;
		
		public EntityRef this[Entity entity] => new EntityRef(this, entity);
		
		
		public event Action<Entity> Added;
		public event Action<Entity> Removed;
		
		
		public EntityRef New()
		{
			var components = new TypedCollection<IComponent>();
			while (true) {
				var entity = new Entity(unchecked(_entityIdCounter++));
				if (!_entities.TryAdd(entity, components).HasValue) {
					Added?.Invoke(entity);
					return new EntityRef(this, entity);
				}
			}
		}
		
		public EntityRef New(params IComponent[] components)
		{
			var entity = New();
			// Add all components dynamically using reflection (slow)
			var method = typeof(EntityRefExtensions).GetTypeInfo()
				.GetMethod(nameof(EntityRefExtensions.Set));
			foreach (var component in components)
				method.MakeGenericMethod(typeof(IEntityRef), component.GetType())
					.Invoke(null, new object[]{ entity, component });
			return entity;
		}
		
		public void Remove(Entity entity) {
			_entities.TryRemove(entity).Expect(
				() => new EntityNonExistantException(this, entity));
			Removed?.Invoke(entity);
		}
		
		
		// IEnumerable implementation
		
		public IEnumerator<EntityRef> GetEnumerator() =>
			_entities.Keys.Select((entity) => this[entity]).GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
