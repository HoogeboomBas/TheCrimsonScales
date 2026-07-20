using Godot;

public partial class VoidSightViewEye : Control
{
	[Export]
	private Sprite2D _pupil;
	[Export]
	private Node2D _pupilContainer;
	[Export]
	private AnimationPlayer _animationPlayer;

	public void Open()
	{
		_animationPlayer.Play("RESET");
		_animationPlayer.Play("open");
		float animationSpeed = AppController.Instance.GameplayTimeScale;
		_animationPlayer.SetSpeedScale(2f * animationSpeed);

		this.DelayedCall(Close, 4f / animationSpeed);
	}

	private void Close()
	{
		_animationPlayer.Play("close");
	}
}