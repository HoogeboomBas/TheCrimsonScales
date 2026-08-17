using Godot;

public partial class Trailer : Node
{
	[Export]
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		base._Ready();

		this.DelayedCall(() =>
		{
			_animationPlayer.Play("title_sequence");
		}, 10f);
	}
}