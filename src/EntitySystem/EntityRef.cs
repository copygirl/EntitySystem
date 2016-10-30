using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Utility;

namespace EntitySystem
{
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
			_manager._entities[_entity].Map((components) => components.TryGet<T>());
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent
		{
			var previous = _manager._entities[_entity]
				.Expect(() => new EntityNonExistantException(_manager, _entity))
				.Set(value);
			if (value.HasValue) {
				if (!previous.HasValue) {
					// Component Added
				}
			} else if (previous.HasValue) {
				// Component removed
			}
			return previous;
		}
		
		public Option<T> Remove<T>() where T : IComponent =>
			Set(Option<T>.None);
		
		// ToString / Casting
		
		public override string ToString() =>
			$"[EntityRef { _entity.ID }]";
		
		public static implicit operator Entity(EntityRef entityRef) =>
			entityRef.Entity.Expect(() => new InvalidOperationException());
	}
}
