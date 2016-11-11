using System;
using System.Diagnostics;
using EntitySystem.Components;
using EntitySystem.Utility;

namespace EntitySystem.Playground
{
	public class Game
	{
		public static void Main() => new Game().Run();
		
		
		char[] _buffer;
		
		public EntityManager Entities { get; private set; }
		public ComponentManager Components { get; private set; }
		
		public Prototype Wall { get; private set; }
		
		
		public void Setup()
		{
			Entities   = new EntityManager();
			Components = Entities.Components;
			
			Wall = new Prototype(Entities.New(
				new Blocking(), new Drawable('#')));
			
			for (var x = 4; x <= 14; x++) {
				Entities.New(Wall, new Position(x, 4));
				Entities.New(Wall, new Position(x, 12));
			}
			for (var y = 5; y < 12; y++) {
				Entities.New(Wall, new Position( 4, y));
				Entities.New(Wall, new Position(14, y));
			}
			
			Console.OutputEncoding = System.Text.Encoding.Unicode;
			Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
			
			var width  = Console.BufferWidth;
			var height = Console.BufferHeight - 1;
			_buffer = new char[width * height];
		}
		
		public void Run()
		{
			Setup();
			
			while (true) {
				Render();
				
				var key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Escape) break;
			}
		}
		
		public void Render()
		{
			_buffer.Fill<char>('.');
			
			var width  = Console.BufferWidth;
			var height = Console.BufferHeight - 1;
			
			foreach (var entry in Components.OfType<Drawable>().GetEntries()) {
				var entity = Entities[entry.Entity];
				var chr    = entry.Component.Character;
				Position pos; if (!entity.Get<Position>().TryGet(out pos)) continue;
				BufferChar(pos.X, pos.Y, chr);
			}
			
			BufferBox(0, 0, width, height);
			
			Console.SetCursorPosition(0, 0);
			Console.Write(_buffer);
			Console.Write("> ");
		}
		
		
		// Buffer Utility Methods
		
		int BufferIndex(int x, int y) => x + y * Console.BufferWidth;
		
		void BufferChar(int x, int y, char chr) =>
			_buffer[BufferIndex(x, y)] = chr;
		
		void BufferLineHor(int x, int y, int width, char chr)
			{ for (var i = 0; i < width; i++) BufferChar(x + i, y, chr); }
		
		void BufferLineVer(int x, int y, int height, char chr)
			{ for (var i = 0; i < height; i++) BufferChar(x, y + i, chr); }
		
		void BufferBox(int x, int y, int width, int height)
		{
			BufferChar(x, y, '╔');
			BufferChar(x + width - 1, y, '╗');
			BufferChar(x, y + height - 1, '╚');
			BufferChar(x + width - 1, y + height - 1, '╝');
			BufferLineHor(x + 1, y, width - 2, '═');
			BufferLineHor(x + 1, y + height - 1, width - 2, '═');
			BufferLineVer(x, y + 1, height - 2, '║');
			BufferLineVer(x + width - 1, y + 1, height - 2, '║');
		}
		
		
		public struct Position : IComponent
		{
			public int X { get; }
			public int Y { get; }
			public Position(int x, int y) { X = x; Y = y; }
			public override string ToString() => $"[{ nameof(Position) } { X }:{ Y }]";
		}
		
		public struct Drawable : IComponent
		{
			public char Character { get; }
			public Drawable(char chr) { Character = chr; }
			public override string ToString() => $"[{ nameof(Drawable) } '{ Character }']";
		}
		
		public struct Blocking : IComponent
		{
			public bool Solid { get; }
			public Blocking(bool solid = true) { Solid = solid; }
			public override string ToString() => $"[{ nameof(Blocking) } Solid={ Solid }]";
		}
	}
}
