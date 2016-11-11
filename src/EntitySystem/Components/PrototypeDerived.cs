using System.Collections;
using System.Collections.Generic;

namespace EntitySystem.Components
{
	public class PrototypeDerived : IComponent, IEnumerable<Entity>
	{
		readonly HashSet<Entity> _derivatives = new HashSet<Entity>();
		
		public void Has(Entity entity) => _derivatives.Contains(entity);
		public void Add(Entity entity) => _derivatives.Add(entity);
		public void Remove(Entity entity) => _derivatives.Remove(entity);
		public void Clear() => _derivatives.Clear();
		
		public IEnumerator<Entity> GetEnumerator() => _derivatives.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _derivatives.GetEnumerator();
	}
}
