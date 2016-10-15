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
		
		
		/// <summary> Returns if the entity has a component of type <typeparam name="T">. </summary>
		bool Has<T>() where T : IComponent;
		
		/// <summary> Gets the component of type <typeparam name="T"> of this entity. </summary>
		Option<T> Get<T>() where T : IComponent;
		
		/// <summary> Sets the component of type <typeparam name="T">
		///           from this entity, returning the previous value.
		///           This will hide the prototype's component value. </summary>
		Option<T> Set<T>(Option<T> value) where T : IComponent;
		
		/// <summary> Removes the component of type <typeparam name="T">
		///           from this entity, returning the previous value.
		///           This will restore the prototype's component value. </summary>
		Option<T> Remove<T>() where T : IComponent;
	}
	
	public static class EntityRefExtensions
	{
		/// <summary> Sets the component of type <typeparam name="T"> from this entity,
		///           returning the previous value. This will hide the prototype's component value.
		///           This is a utility extension method to avoid specifying the type parameter. </summary>
		public static Option<T> Set<E, T>(this E entity, T value)
			where E : IEntityRef where T : IComponent =>
			entity.Set(Option<T>.Some(value));
	}
}
