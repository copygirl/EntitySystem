using System;
using System.Collections.Generic;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class EntityRef : IEntityRef
	{
		readonly EntityManager _manager;
		readonly Entity _entity;
		
		public bool Exists => _manager.Has(_entity);
		
		internal EntityRef(EntityManager manager, Entity entity)
		{
			_manager = manager;
			_entity = entity;
		}
		
		// IEntityRef implementation
		
		public Option<Entity> Entity => new Option<Entity>(_entity, Exists);
		
		public IEnumerable<IComponent> Components =>
			_manager.Components.GetAll(_entity);
		
		public Option<T> Get<T>() where T : IComponent =>
			_manager.Components.Get<T>(_entity);
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent =>
			_manager.Components.Set<T>(_entity, value);
		
		// ToString / Casting
		
		public override string ToString() =>
			$"[{ nameof(EntityRef) } { _entity.ID }]";
		
		public static implicit operator Entity(EntityRef entityRef) =>
			entityRef.Entity.Expect(() => new InvalidOperationException());
	}
}
