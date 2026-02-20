using Godot;

public partial class CharacterTokenHexObjectEffectView : HexObjectEffectView<CharacterTokenHexObjectEffectView.Parameters>
{
	public class Parameters(Character owner, object subscriber) : HexObjectEffectViewParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/HexObjectEffectViews/CharacterTokenHexObjectEffectView.tscn";

		public Character Owner { get; } = owner;
		public object Subscriber { get; } = subscriber;
	}

	[Export]
	private Sprite2D _sprite;

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_sprite.SetTexture(parameters.Owner.ClassModel.CharacterTokenTexture);
	}
}