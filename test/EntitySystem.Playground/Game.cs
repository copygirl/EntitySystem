using System;
using System.Diagnostics;
using System.Linq;
using EntitySystem.Components;
using EntitySystem.Playground.Components;

namespace EntitySystem.Playground
{
	public class Game
	{
		public static void Main() => new Game().Run();
		
		
		public EntityManager Entities { get; private set; }
		public ComponentManager Components { get; private set; }
		
		public LevelMap Level { get; private set; }
		public ConsoleBuffer Buffer { get; private set; }
		
		public Prototype Wall { get; private set; }
		
		public IEntityRef Player { get; private set; }
		
		
		public void Setup()
		{
			Entities   = new EntityManager();
			Components = Entities.Components;
			
			Level  = new LevelMap(this);
			Buffer = new ConsoleBuffer();
			
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
			
			Player = Entities.New(
				new Blocking(), new Position(6, 6), new Drawable('@'));
		}
		
		public void Run()
		{
			Setup();
			
			var running = true;
			while (running) {
				Render();
				
				switch (Console.ReadKey(true).Key) {
					case ConsoleKey.LeftArrow:  Move(-1,  0); break;
					case ConsoleKey.RightArrow: Move( 1,  0); break;
					case ConsoleKey.UpArrow:    Move( 0, -1); break;
					case ConsoleKey.DownArrow:  Move( 0,  1); break;
					case ConsoleKey.Escape: running = false; break;
				}
			}
		}
		
		void Move(int x, int y)
		{
			Position pos; if (!Player.Get<Position>().TryGet(out pos)) return;
			var target = new Position(pos.X + x, pos.Y + y);
			
			// If there's any blocking entities in the way, ...
			if (Level[target].Any(
				(entity) => (entity.Get<Blocking>()
					.Map((blocking) => blocking.Solid) == true))) return;
			
			Player.Set(target);
		}
		
		public void Render()
		{
			Buffer.Clear();
			
			var camera = Player.Get<Position>().Or(new Position(0, 0));
			
			foreach (var entry in Components.OfType<Drawable>().GetEntries()) {
				var entity = Entities[entry.Entity];
				var chr    = entry.Component.Character;
				Position pos; if (!entity.Get<Position>().TryGet(out pos)) continue;
				
				pos = pos.Add(Buffer.Width / 2, Buffer.Height / 2) - camera; 
				if ((pos.X < 1) || (pos.X >= Buffer.Width - 1) ||
				    (pos.Y < 1) || (pos.Y >= Buffer.Height - 1)) continue;
				
				Buffer.Char(pos.X, pos.Y, chr);
			}
			
			Buffer.Box(0, 0, Buffer.Width, Buffer.Height);
			Buffer.Flush();
		}
	}
}
