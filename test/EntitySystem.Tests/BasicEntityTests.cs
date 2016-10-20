using System.Linq;
using Xunit;

namespace EntitySystem.Tests
{
	public class BasicEntityTests
	{
		public EntityManager Entities { get; } = new EntityManager();
		
		[Fact]
		public void TestSingleEntityAndComponent()
		{
			// Adding entity
			Assert.Equal(Entities.Count(), 0);
			var entity = Entities.New();
			Assert.Equal(Entities.Count(), 1);
			
			// Adding component
			Assert.Equal(entity.Components.Count(), 0);
			entity.Set(new TestComponent(10));
			Assert.Equal(entity.Components.Count(), 1);
			
			// Getting component
			Assert.Equal(entity.Get<TestComponent>(), new TestComponent(10));
			Assert.Equal(entity.Components.First(), new TestComponent(10));
			
			// Removing component
			entity.Remove<TestComponent>();
			Assert.Equal(entity.Components.Count(), 0);
			
			// Removing entity
			Entities.Remove(entity);
			Assert.Equal(Entities.Count(), 0);
		}
		
		struct TestComponent : IComponent
		{
			public int Value { get; private set; }
			public TestComponent(int value) { Value = value; }
		}
	}
}
