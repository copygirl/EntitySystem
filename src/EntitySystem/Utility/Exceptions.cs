using System;

namespace EntitySystem.Utility
{
	public class EntityNonExistantException : Exception
	{
		public EntityManager Manager { get; }
		public Entity Entity { get; }
		
		public EntityNonExistantException(EntityManager manager, Entity entity)
			: base($"{ entity } does not exist in { nameof(EntityManager) }")
		{
			Manager = manager;
			Entity = entity;
		}
	}
}
