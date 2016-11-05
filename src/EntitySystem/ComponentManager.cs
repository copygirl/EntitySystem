using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitySystem.Collections;
using EntitySystem.Components;
using EntitySystem.Storage;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class ComponentManager
	{
		readonly TypedCollection<object> _typeHandlers = new TypedCollection<object>();
		readonly GenericComponentMap _defaultMap = new GenericComponentMap();
		
		public EntityManager Entities { get; }
		
		public event Action<Entity, IComponent> Added;
		public event Action<Entity, IComponent> Removed;
		
		
		internal ComponentManager(EntityManager entities)
		{
			Entities = entities;
			Entities.Removed += _defaultMap.RemoveAll; // This might be slow..?
		}
		
		
		public ComponentsOfType<T> OfType<T>() where T : IComponent =>
			_typeHandlers.GetOrAdd<ComponentsOfType<T>>(() => {
				var type = typeof(T).GetTypeInfo();
				if (type.IsInterface || type.IsAbstract)
					throw new InvalidOperationException(
						$"{ typeof(T) } is not a concrete component type");
				return new ComponentsOfType<T>(this);
			});
		
		
		public Option<T> Get<T>(Entity entity) where T : IComponent
		{
			if (!Entities.Has(entity)) throw new EntityNonExistantException(Entities, entity);
			return _defaultMap.Get<T>(entity).Or(() =>
				(typeof(T) != typeof(Prototype))
					? _defaultMap.Get<Prototype>(entity)
						.Map((prototype) => Get<T>(prototype))
					: Option<T>.None);
		}
		
		public Option<T> Set<T>(Entity entity, Option<T> valueOption) where T : IComponent
		{
			if (!Entities.Has(entity)) throw new EntityNonExistantException(Entities, entity);
			
			var previousOption = _defaultMap.Set<T>(entity, valueOption);
			T value; var hasValue = valueOption.TryGet(out value);
			T previous; var hasPrevious = previousOption.TryGet(out previous);
			
			if (hasValue) {
				if (!hasPrevious) {
					OfType<T>().RaiseAdded(entity, value);
					Added?.Invoke(entity, value);
				}
			} else if (hasPrevious) {
				OfType<T>().RaiseRemoved(entity, previous);
				Removed?.Invoke(entity, previous);
			}
			
			return previousOption;
		}
		
		public IEnumerable<IComponent> GetAll(Entity entity)
		{
			if (!Entities.Has(entity)) throw new EntityNonExistantException(Entities, entity);
			return _defaultMap.GetAll(entity).Concat(
				_defaultMap.Get<Prototype>(entity)
					.Map((prototype) => GetAll(prototype))
					.Or(Enumerable.Empty<IComponent>()));
		}
		
		
		public class ComponentsOfType<T> where T : IComponent
		{
			readonly ComponentManager _manager;
			
			public event Action<Entity, T> Added;
			public event Action<Entity, T> Removed;
			// Add Changed event? It's possible. Unsure if needed.
			// Depends on how large the overhead for it would be.
			// Can be overridden by using a custom storage handler in the future.
			
			public IEnumerable<EntityComponentEntry<T>> Entries =>
				_manager._defaultMap.Entries<T>();
			
			internal ComponentsOfType(ComponentManager manager) { _manager = manager; }
			
			internal void RaiseAdded(Entity entity, T component) => Added?.Invoke(entity, component);
			internal void RaiseRemoved(Entity entity, T component) => Removed?.Invoke(entity, component);
		}
	}
}
