using System;
using System.Linq;
using EntitySystem.Components.World;
using EntitySystem.Utility;
using EntitySystem.World;
using Xunit;
using Xunit.Abstractions;

namespace EntitySystem.Tests
{
	public class BasicChunkTests
	{
		public EntityManager Entities { get; }
		public ChunkManager Chunks { get; }
		public ITestOutputHelper Output { get; }
		
		public BasicChunkTests(ITestOutputHelper output)
		{
			Entities = new EntityManager();
			Chunks = new ChunkManager(Entities);
			Output = output;
		}
		
		[Fact]
		public void Test()
		{
			Chunks.Storage.Register<TestChunkComponent>();
			
			var pos = new BlockPos(100, 200, 300);
			
			var block = Chunks.Get(pos);
			Assert.Equal(block.Components.Count(), 1);
			Assert.True(block.Has<Block>());
			Assert.Equal(block.Get<Block>().Value.Position, pos);
			
			var chunk = Chunks.GetChunk(pos);
			Assert.Equal(chunk.Components.Count(), 1);
			Assert.True(chunk.Has<Chunk>());
			Assert.Equal(chunk.Get<Chunk>().Value.Position, Chunks.GetChunkPos(pos));
			
			// Make sure there's no entities right now
			Assert.Equal(Entities.Count(), 0);
			
			// Set and test a (registered) block component
			TestSetBlockRefComponent(block, new TestChunkComponent(10));
			Assert.False(block.Entity.HasValue); // No block entity,
			Assert.Equal(Entities.Count(), 1);   // but a chunk entity
			Assert.True(chunk.Has<ChunkBlockStorage<TestChunkComponent>>());
			
			// Set and test a non-registered component
			TestSetBlockRefComponent(block, new TestEntityComponent(10));
			Assert.True(block.Entity.HasValue); // This should've created
			Assert.Equal(Entities.Count(), 2);  // a new block entity
			Assert.True(chunk.Has<ChunkBlockEntities>());
			
			Assert.Equal(block.Components.OrderBy(NameSelector),
				new IComponent[] {
					new Block(pos),
					new TestChunkComponent(10),
					new TestEntityComponent(10)
				}.OrderBy(NameSelector));
		}
		
		String NameSelector(IComponent component) => component.GetType().Name;
		
		void TestSetBlockRefComponent<T>(BlockRef block, T component) where T : IComponent
		{
			Assert.False(block.Has<T>());
			Assert.Equal(block.Get<T>(), Option<T>.None);
			Assert.DoesNotContain(component, block.Components);
			
			block.Set(component);
			
			Assert.True(block.Has<T>());
			Assert.Equal(block.Get<T>(), component);
			Assert.Contains(component, block.Components);
		}
		
		struct TestEntityComponent : IComponent
		{
			public int Value { get; private set; }
			public TestEntityComponent(int value) { Value = value; }
		}
		
		struct TestChunkComponent : IComponent
		{
			public byte Value { get; private set; }
			public TestChunkComponent(byte value) { Value = value; }
		}
	}
}
