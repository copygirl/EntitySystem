using System.Collections.Generic;
using EntitySystem.Utility;

namespace EntitySystem
{
	/// <summary>
	/// Represents a reference to an entity managed by an EntityManager,
	/// allowing for read / write access to its components.
	/// 
	/// Note that keeping around an instance of this type may result in
	/// exceptions or unexpected behavior when accessing components if
	/// the entity doesn't exist anymore or the Entity value is reused.
	/// </summary>
	public struct EntityRef : IEntityRef
	{
		readonly EntityManager _manager;
		readonly Entity _entity;
		
		// FIXME: Exists may return true if a new entity
		//        is recreated using the same Entity key.
		public bool Exists => _manager.Has(_entity);
		
		internal EntityRef(EntityManager manager, Entity entity)
		{
			_manager = manager;
			_entity = entity;
		}
		
		// IEntityRef implementation
		
		public Option<Entity> Entity => new Option<Entity>(_entity, Exists);
		
		public IEnumerable<IComponent> Components { get {
			if (!Exists) throw new EntityNonExistantException(_manager, _entity);
			return _manager.Components.GetAll(_entity);
		} }
		
		public Option<T> Get<T>() where T : IComponent {
			if (!Exists) throw new EntityNonExistantException(_manager, _entity);
			return _manager.Components.Get<T>(_entity);
		}
		
		public Option<T> Set<T>(Option<T> value) where T : IComponent {
			if (!Exists) throw new EntityNonExistantException(_manager, _entity);
			return _manager.Components.Set<T>(_entity, value);
		}
		
		// ToString / Casting
		
		public override string ToString() => $"[{ nameof(EntityRef) } { _entity.ID }]";
		
		public static implicit operator Entity(EntityRef entityRef) =>
			entityRef.Entity.OrDefault();
	}
}
