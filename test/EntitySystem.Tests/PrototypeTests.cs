using System.Linq;
using EntitySystem.Components;
using Xunit;

namespace EntitySystem.Tests
{
	public class PrototypeTests
	{
		public EntityManager Entities { get; } = new EntityManager();
		
		[Fact]
		public void TestPrototypeSimple()
		{
			var prototype = Entities.New(new TestComponent(10));
			var derived = Entities.New(new Prototype(prototype));
			Assert.Equal(derived.Get<TestComponent>().Value, new TestComponent(10));
		}
		
		[Fact]
		public void TestPrototypeMulti()
		{
			var prototype = Entities.New(new TestComponent(10));
			var derived1 = Entities.New(new Prototype(prototype));
			var derived2 = Entities.New(new Prototype(derived1));
			var derived3 = Entities.New(new Prototype(derived2));
			Assert.Equal(derived3.Get<TestComponent>().Value, new TestComponent(10));
		}
		
		[Fact]
		public void TestPrototypeOverride()
		{
			var prototype = Entities.New(new TestComponent(10));
			var derived = Entities.New(new Prototype(prototype));
			derived.Set(new TestComponent(20));
			Assert.Equal(prototype.Get<TestComponent>().Value, new TestComponent(10));
			Assert.Equal(derived.Get<TestComponent>().Value, new TestComponent(20));
		}
		
		[Fact]
		public void TestPrototypeInternals()
		{
			var prototype = Entities.New(new TestComponent(10));
			var child1 = Entities.New(new Prototype(prototype));
			var child2 = Entities.New(new Prototype(prototype));
			var child3 = Entities.New(new Prototype(prototype));
			
			var derived = prototype.Get<PrototypeDerived>().Value;
			
			// Verify that the PrototypeDerived component
			// contains all 3 derived ("child") entities.
			Assert.True(derived.Has(child1));
			Assert.True(derived.Has(child2));
			Assert.True(derived.Has(child3));
			Assert.Equal(derived.Count(), 3);
			
			Entities.Remove(child1);
			Entities.Remove(child2);
			
			// Verify that the derived entities were removed.
			Assert.False(derived.Has(child1));
			Assert.False(derived.Has(child2));
			Assert.True(derived.Has(child3));
			Assert.Equal(derived.Count(), 1);
		}
		
		struct TestComponent : IComponent
		{
			public int Value { get; private set; }
			public TestComponent(int value) { Value = value; }
		}
	}
}
