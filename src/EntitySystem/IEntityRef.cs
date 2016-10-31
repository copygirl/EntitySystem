using System;
using System.Collections.Generic;
using EntitySystem.Utility;

namespace EntitySystem
{
	public interface IEntityRef
	{
		/// <summary> Returns the Entity value associated with this entity reference, if any. </summary>
		Option<Entity> Entity { get; }
		
		/// <summary> Returns an enumerable of all of this entity's components. </summary>
		IEnumerable<IComponent> Components { get; }
		
		
		/// <summary> Gets the component of type <typeparam name="T"> of this entity. </summary>
		Option<T> Get<T>() where T : IComponent;
		
		/// <summary> Sets the component of type <typeparam name="T">
		///           of this entity, returning the previous value. </summary>
		Option<T> Set<T>(Option<T> value) where T : IComponent;
	}
	
	public static class EntityRefExtensions
	{
		/// <summary> Returns if the entity has a component of type <typeparam name="T">. </summary>
		public static bool Has<T>(this IEntityRef entity) where T : IComponent =>
			entity.Get<T>().HasValue;
		
		
		/// <summary> Gets the component of type <typeparam name="T"> of this entity
		///           or adds and returns the default value if it doesn't exist. </summary>
		public static T GetOrAdd<T>(this IEntityRef entity, T @default) where T : IComponent =>
			entity.Get<T>().Or(() => { entity.Set<T>(@default); return @default; });
		
		/// <summary> Gets the component of type <typeparam name="T"> of this entity
		///           or adds and returns a default value if it doesn't exist. </summary>
		public static T GetOrAdd<T>(this IEntityRef entity, Func<T> defaultFactory) where T : IComponent =>
			entity.Get<T>().Or(() => { var value = defaultFactory(); entity.Set<T>(value); return value; });
		
		
		/// <summary> Sets the component of type <typeparam name="T">
		///           of this entity, returning the previous value.
		///           (Utility method, helps to avoid specifying the type parameter.) </summary>
		public static Option<T> Set<T>(this IEntityRef entity, T value) where T : IComponent =>
			entity.Set(Option<T>.Some(value));
		
		/// <summary> Removes the component of type <typeparam name="T">
		///           of this entity, returning the previous value. </summary>
		public static Option<T> Remove<T>(this IEntityRef entity) where T : IComponent =>
			entity.Set(Option<T>.None);
	}
}
