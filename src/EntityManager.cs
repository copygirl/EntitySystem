using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitySystem.Collections;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class EntityManager
	{
		readonly OptionDictionary<Entity, TypedCollection<IComponent>> _entities =
			new OptionDictionary<Entity, TypedCollection<IComponent>>();
		
		uint _entityIdCounter = 1;
		
		
		public IEnumerable<Entity> Entities => _entities.Keys.Select(x => x);
		
		public EntityRef this[Entity entity] => new EntityRef(this, entity);
		
		
		public EntityRef New()
		{
			var components = new TypedCollection<IComponent>();
			while (true) {
				var entity = new Entity(unchecked(_entityIdCounter++));
				if (!_entities.TryAdd(entity, components).HasValue)
					return new EntityRef(this, entity);
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
		
		public void Remove(Entity entity) =>
			_entities.TryRemove(entity).Expect(
				() => new EntityNonExistantException(entity));
		
		
		public class EntityRef : IEntityRef
		{
			readonly EntityManager _manager;
			readonly Entity _entity;
			
			public bool Exists => _manager._entities[_entity].HasValue;
			
			internal EntityRef(EntityManager manager, Entity entity)
				{ _manager = manager; _entity = entity; }
			
			// IEntityRef implementation
			
			public Option<Entity> Entity => new Option<Entity>(_entity, Exists);
			
			public IEnumerable<IComponent> Components =>
				_manager._entities[_entity]
					.Map((components) => components.Select(x => x))
					.Or(Enumerable.Empty<IComponent>());
			
			public bool Has<T>() where T : IComponent =>
				Get<T>().HasValue;
			
			public Option<T> Get<T>() where T : IComponent =>
				_manager._entities[_entity].Map((components) => components.Get<T>());
			
			public Option<T> Set<T>(Option<T> value) where T : IComponent =>
				_manager._entities[_entity].Expect(
						() => new EntityNonExistantException(_entity)
					).Set(value);
			
			public Option<T> Remove<T>() where T : IComponent =>
				Set(Option<T>.None);
			
			// ToString / Casting
			
			public override string ToString() =>
				_entity.ToString().Replace(nameof(_entity), nameof(EntityRef));
			
			public static implicit operator Entity(
				EntityRef entityRef) => entityRef._entity;
		}
		
		public class EntityNonExistantException : Exception
		{
			public Entity Entity { get; private set; }
			
			public EntityNonExistantException(Entity entity)
				: base($"{ entity } does not exist") { Entity = entity; }
		}
	}
}
