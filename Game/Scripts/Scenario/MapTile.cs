using Godot;

public partial class MapTile : Node2D
{
	[Export]
	private Node2D _hexParent;
	[Export]
	private Sprite2D _sprite;
	[Export]
	private Color _lockedColor;
	[Export]
	public SFX.StepType StepType;

	public Sprite2D Sprite => _sprite;

	public void Init()
	{
		_sprite.Reparent(GameController.Instance.Map);
		_sprite.GetParent().MoveChild(_sprite, 0);

		_sprite.SelfModulate = _lockedColor;
	}

	public void Reveal()
	{
		_sprite.SelfModulate = Colors.White;
	}
}