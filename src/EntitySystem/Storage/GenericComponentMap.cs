using System;
using System.Collections.Generic;
using System.Linq;
using EntitySystem.Collections;
using EntitySystem.Utility;

namespace EntitySystem.Storage
{
	public class GenericComponentMap
	{
		OptionDictionary<Type, IEntityComponentMap> _dict =
			new OptionDictionary<Type, IEntityComponentMap>();
		
		
		public Option<T> Get<T>(Entity entity) where T : IComponent =>
			_dict[typeof(T)]
				.Cast<EntityComponentMap<T>>()
				.Map((map) => map.TryGet(entity));
		
		public Option<T> Set<T>(Entity entity, Option<T> value) where T : IComponent =>
			((EntityComponentMap<T>)_dict.GetOrAdd(typeof(T),
					(_) => new EntityComponentMap<T>()))
				.Set(entity, value);
		
		
		public IEnumerable<EntityComponentEntry<T>> Entries<T>() where T : IComponent =>
			_dict[typeof(T)]
				.Cast<EntityComponentMap<T>>()
				.Map((map) => map.Select((entry) =>
					new EntityComponentEntry<T>(entry.Key, entry.Value)))
				.Or(Enumerable.Empty<EntityComponentEntry<T>>());
		
		
		public IEnumerable<IComponent> GetAll(Entity entity) =>
			_dict.Values.SelectSome((map) => map.TryGet(entity));
		
		public void RemoveAll(Entity entity)
		{
			foreach (var map in _dict.Values)
				map.TryRemove(entity);
		}
		
		
		interface IEntityComponentMap
		{
			Option<IComponent> TryGet(Entity entity);
			
			void TryRemove(Entity entity);
		}
		
		class EntityComponentMap<T> : OptionDictionary<Entity, T>, IEntityComponentMap
			where T : IComponent
		{
			Option<IComponent> IEntityComponentMap.TryGet(Entity entity) =>
				base.TryGet(entity).Cast<IComponent>();
			
			void IEntityComponentMap.TryRemove(Entity entity) =>
				base.TryRemove(entity);
		}
	}
}
