using Godot;

public partial class SpiritViewComponent : HexObjectViewComponent
{
	[Export]
	public Sprite2D StandeeNumberCircle { get; private set; }

	[Export]
	public Label StandeeNumberLabel { get; private set; }

	[Export]
	public Sprite2D Sprite { get; private set; }
}