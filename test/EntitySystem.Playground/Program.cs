using System;
using EntitySystem.Components.World;
using EntitySystem.World;

namespace EntitySystem.Playground
{
	public class Program
	{
		public static void Main()
		{
			var entities = new EntityManager();
			var chunks   = new ChunkManager(entities);
			
			chunks.Storage.Register<Test>();
			
			var pos         = BlockPos.ORIGIN;
			var chunkPos    = chunks.GetChunkPos(pos);
			var chunkRelPos = chunks.GetChunkRelPos(pos);
			
			var block = chunks.Get(pos);
			var chunk = chunks.GetChunk(chunkPos);
			block.Set(new Test(10));
			
			Console.WriteLine($"Chunk: { string.Join(", ", chunk.Components) }");
			Console.WriteLine($"Block: { string.Join(", ", block.Components) }");
			
			var test = chunk.Get<ChunkBlockStorage<Test>>().Value.GetDirect(BlockPos.ORIGIN);
			Console.WriteLine($"Block.TestComponent: { test }");
		}
		
		struct Test : IComponent
		{
			public byte Value { get; }
			public Test(byte value) { Value = value; }
			public override string ToString() => $"[Test { Value }]";
		}
	}
}
