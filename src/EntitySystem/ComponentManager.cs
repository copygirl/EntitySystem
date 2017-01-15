using System;
using System.Collections.Generic;
using System.Reflection;
using EntitySystem.Collections;
using EntitySystem.Components;
using EntitySystem.Storage;
using EntitySystem.Utility;

namespace EntitySystem
{
	public class ComponentManager
	{
		readonly OptionDictionary<Type, IComponentMap> _handlers =
			new OptionDictionary<Type, IComponentMap>();
		
		public EntityManager Entities { get; }
		
		public delegate void ComponentAdded<T>(Entity entity, T value) where T : IComponent;
		public delegate void ComponentRemoved<T>(Entity entity, T previous) where T : IComponent;
		public delegate void ComponentChanged<T>(Entity entity, Option<T> previous, Option<T> current) where T : IComponent;
		
		public event ComponentAdded<IComponent> Added;
		public event ComponentRemoved<IComponent> Removed;
		public event ComponentChanged<IComponent> Changed;
		
		
		internal ComponentManager(EntityManager entities)
		{
			Entities = entities;
			
			Entities.Removed += (entity) => {
				foreach (var handler in _handlers.Values)
					handler.TryRemove(entity);
			};
			
			OfType<Prototype>().Changed += (entity, previousOption, currentOption) => {
				Prototype previous; if (previousOption.TryGet(out previous)) {
					PrototypeDerived derivatives;
					if (Get<PrototypeDerived>(previous).TryGet(out derivatives))
						derivatives.Remove(entity);
				}
				Prototype current; if (currentOption.TryGet(out current))
					GetOrAdd(current, () => new PrototypeDerived()).Add(entity);
			};
		}
		
		
		ComponentHandler<T> GetHandler<T>() where T : IComponent =>
			(ComponentHandler<T>)_handlers.GetOrAdd(typeof(T), (type) => {
				var info = typeof(T).GetTypeInfo();
				if (info.IsInterface || info.IsAbstract)
					throw new InvalidOperationException(
						$"{ typeof(T) } is not a concrete component type");
				return new ComponentHandler<T>(this);
			});
		
		public IComponentsOfType<T> OfType<T>() where T : IComponent =>
			(IComponentsOfType<T>)GetHandler<T>();
		
		
		public Option<T> GetPersonal<T>(Entity entity) where T : IComponent =>
			GetHandler<T>().TryGet(entity);
		
		public Option<T> Get<T>(Entity entity) where T : IComponent =>
			GetPersonal<T>(entity).Or(() =>
				(typeof(T) != typeof(Prototype))
					? GetPersonal<Prototype>(entity)
						.Map((prototype) => Get<T>(prototype))
					: Option<T>.None);
		
		public T GetOrAdd<T>(Entity entity, Func<T> defaultFactory) where T : IComponent =>
			Get<T>(entity).Or(() => {
				var value = defaultFactory();
				Set<T>(entity, value);
				return value;
			});
		
		public IEnumerable<IComponent> GetAll(Entity entity)
		{
			// TODO: Handle prototype values.
			return _handlers.Values.SelectSome((map) => map.TryGet(entity));
		}
		
		public Option<T> Set<T>(Entity entity, T value) where T : IComponent =>
			Set(entity, Option<T>.Some(value));
		
		public Option<T> Set<T>(Entity entity, Option<T> value) where T : IComponent
		{
			var handler = GetHandler<T>();
			var previous = handler.Set(entity, value);
			RaiseChanged(handler, entity, previous, value);
			return previous;
		}
		
		
		void RaiseChanged<T>(ComponentHandler<T> handler, Entity entity,
		                     Option<T> previousOption, Option<T> currentOption) where T : IComponent
		{
			T current; var hasValue = currentOption.TryGet(out current);
			T previous; var hasPrevious = previousOption.TryGet(out previous);
			
			if (hasValue) {
				if (!hasPrevious) {
					handler.RaiseAdded(entity, current);
					Added?.Invoke(entity, current);
				}
			} else if (hasPrevious) {
				handler.RaiseRemoved(entity, previous);
				Removed?.Invoke(entity, previous);
			}
			
			handler.RaiseChanged(entity, previousOption, currentOption);
			Changed?.Invoke(entity, previousOption.Cast<IComponent>(), currentOption.Cast<IComponent>());
		}
		
		
		public interface IComponentsOfType<T> where T : IComponent
		{
			event ComponentAdded<T> Added;
			event ComponentRemoved<T> Removed;
			event ComponentChanged<T> Changed;
			
			IEnumerable<EntityComponentEntry<T>> GetEntries(LookupMode mode = LookupMode.Concrete);
		}
		
		public enum LookupMode
		{
			Personal,
			Concrete,
			All
		}
		
		
		interface IComponentMap
		{
			Option<IComponent> TryGet(Entity entity);
			
			void TryRemove(Entity entity);
		}
		
		class ComponentHandler<T> : OptionDictionary<Entity, T>,
		                            IComponentMap, IComponentsOfType<T>
			where T : IComponent
		{
			readonly ComponentManager _manager;
			
			internal ComponentHandler(ComponentManager manager) { _manager = manager; }
			
			internal void RaiseAdded(Entity entity, T value) =>
				Added?.Invoke(entity, value);
			internal void RaiseRemoved(Entity entity, T previous) =>
				Removed?.Invoke(entity, previous);
			internal void RaiseChanged(Entity entity, Option<T> previous, Option<T> current) =>
				Changed?.Invoke(entity, previous, current);
			
			// IComponentsOfType implementation
			
			public event ComponentAdded<T> Added;
			public event ComponentRemoved<T> Removed;
			public event ComponentChanged<T> Changed;
			
			public IEnumerable<EntityComponentEntry<T>> GetEntries(LookupMode mode = LookupMode.Concrete)
			{
				foreach (var entry in this) {
					if (mode == LookupMode.Personal)
						yield return new EntityComponentEntry<T>(entry.Key, entry.Value);
					else foreach (var e in GetEntries(entry.Key, entry.Value, (mode == LookupMode.All)))
						yield return e;
				}
			}
			
			IEnumerable<EntityComponentEntry<T>> GetEntries(Entity entity, T value, bool yieldPrototypes)
			{
				PrototypeDerived derivatives = _manager.GetPersonal<PrototypeDerived>(entity).OrDefault();
				if ((derivatives == null) || yieldPrototypes)
					yield return new EntityComponentEntry<T>(entity, value);
				if (derivatives != null)
					foreach (var derivedEntity in derivatives)
						if (!ContainsKey(derivedEntity))
							foreach (var entry in GetEntries(derivedEntity, value, yieldPrototypes))
								yield return entry;
			}
			
			// IComponentMap implementation
			
			Option<IComponent> IComponentMap.TryGet(Entity entity) =>
				base.TryGet(entity).Cast<IComponent>();
			
			void IComponentMap.TryRemove(Entity entity)
			{
				Option<T> previous = base.TryRemove(entity);
				if (previous.HasValue)
					_manager.RaiseChanged(this, entity, previous, Option<T>.None);
			}
		}
	}
}
