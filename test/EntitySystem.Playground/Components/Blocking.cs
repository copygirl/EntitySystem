namespace EntitySystem.Playground.Components
{
	public struct Blocking : IComponent
	{
		readonly bool _nonSolid;
		public bool Solid => !_nonSolid;
		
		public Blocking(bool solid) { _nonSolid = !solid; }
		
		public override string ToString() => $"[{ nameof(Blocking) } Solid={ Solid }]";
	}
}
