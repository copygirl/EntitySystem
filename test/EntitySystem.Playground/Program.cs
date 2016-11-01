using System;
using EntitySystem.Components;
using EntitySystem.Components.World;
using EntitySystem.Utility;
using EntitySystem.World;

namespace EntitySystem.Playground
{
	public class Program
	{
		public static void Main()
		{
			var entities = new EntityManager();
			var chunks   = new ChunkManager(entities);
			
			chunks.Storage.Register<BlockTest>();
			
			var pos         = BlockPos.ORIGIN;
			var chunkPos    = chunks.GetChunkPos(pos);
			var chunkRelPos = chunks.GetChunkRelPos(pos);
			
			var prototype = entities.New(
				new GenericTest<byte>(),
				new GenericTest<short>(),
				new GenericTest<int>());
			
			var block = chunks.Get(pos);
			var chunk = chunks.GetChunk(chunkPos);
			block.Set(new BlockTest(10));
			
			block.Set(new Prototype(prototype));
			chunk.Set(new Prototype(prototype));
			
			Console.WriteLine($"Chunk: { string.Join(", ", chunk.Components) }");
			Console.WriteLine($"Block: { string.Join(", ", block.Components) }");
			
			var test = chunk.Get<ChunkBlockStorage<BlockTest>>().Value.GetDirect(BlockPos.ORIGIN);
			Console.WriteLine($"Block.TestComponent: { test }");
			
			Console.WriteLine($"All GenericTest<byte>: { string.Join(", ", entities.Components.OfType<GenericTest<byte>>().Entries) }");
			Console.WriteLine($"All GenericTest<short>: { string.Join(", ", entities.Components.OfType<GenericTest<short>>().Entries) }");
			Console.WriteLine($"All GenericTest<int>: { string.Join(", ", entities.Components.OfType<GenericTest<int>>().Entries) }");
		}
		
		struct BlockTest : IComponent
		{
			public byte Value { get; }
			public BlockTest(byte value) { Value = value; }
			public override string ToString() => $"[{ nameof(BlockTest) } { Value }]";
		}
		
		struct GenericTest<T> : IComponent
		{
			public T Value { get; }
			public GenericTest(T value) { Value = value; }
			public override string ToString() => $"[{ GetType().GetFriendlyName() } { Value }]";
		}
	}
}
