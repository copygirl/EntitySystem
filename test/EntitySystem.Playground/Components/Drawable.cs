namespace EntitySystem.Playground.Components
{
	public struct Drawable : IComponent
	{
		public char Character { get; }
		
		public Drawable(char chr) { Character = chr; }
		
		public override string ToString() => $"[{ nameof(Drawable) } '{ Character }']";
	}
}
