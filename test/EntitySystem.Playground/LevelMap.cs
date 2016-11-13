using System.Collections.Generic;
using System.Linq;
using EntitySystem.Collections;
using EntitySystem.Playground.Components;

namespace EntitySystem.Playground
{
	public class LevelMap
	{
		readonly OptionDictionary<Position, HashSet<Entity>> _map =
			new OptionDictionary<Position, HashSet<Entity>>();
		
		public Game Game { get; }
		
		public IEnumerable<EntityRef> this[Position pos] =>
			_map.TryGet(pos)
				.Map((set) => set.Select((entity) => Game.Entities[entity]))
				.Or(Enumerable.Empty<EntityRef>());
		
		public LevelMap(Game game)
		{
			Game = game;
			Game.Components.OfType<Position>().Changed += (entity, previousOption, currentOption) => {
				Position pos;
				if (previousOption.TryGet(out pos)) {
					HashSet<Entity> set; if (_map.TryGet(pos).TryGet(out set)) {
						set.Remove(entity);
						if (set.Count == 0) _map.Remove(pos);
					}
				}
				if (currentOption.TryGet(out pos))
					_map.GetOrAdd(pos, (_) => new HashSet<Entity>()).Add(entity);
			};
		}
	}
}
