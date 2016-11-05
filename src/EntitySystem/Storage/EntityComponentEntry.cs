namespace EntitySystem.Storage
{
	public struct EntityComponentEntry<T> where T : IComponent
	{
		public Entity Entity { get; }
		public T Component { get; }
		
		public EntityComponentEntry(Entity entity, T component)
		{
			Entity = entity;
			Component = component;
		}
	}
}
