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
			
			// Empty entity
			Assert.False(entity.Has<TestComponent>());
			Assert.Equal(entity.Components.Count(), 0);
			
			// Adding component
			entity.Set(new TestComponent(10));
			Assert.Equal(entity.Components.Count(), 1);
			
			// Getting component
			Assert.True(entity.Has<TestComponent>());
			Assert.Equal(entity.Get<TestComponent>().Value, new TestComponent(10));
			Assert.Contains(new TestComponent(10), entity.Components);
			
			// Removing component
			entity.Remove<TestComponent>();
			Assert.False(entity.Has<TestComponent>());
			Assert.Equal(entity.Components.Count(), 0);
			
			// Removing entity
			Entities.Remove(entity);
			Assert.False(entity.Exists);
			Assert.Equal(Entities.Count(), 0);
		}
		
		struct TestComponent : IComponent
		{
			public int Value { get; private set; }
			public TestComponent(int value) { Value = value; }
		}
	}
}
