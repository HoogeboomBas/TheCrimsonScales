using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class ConditionAnimation : Node2D
{
	[Export]
	private Node2D _scaleContainer;
	[Export]
	private AnimationPlayer _animationPlayer;
	[Export]
	private string _sfxPath;
	[Export]
	private float _sfxVolumeDb;
	[Export]
	private float _sfxDelay;
	[Export]
	private float _animationDelay;

	public void Init(Figure figure)
	{
		GlobalPosition = figure.GlobalPosition;

		_animationPlayer.Stop();

		_scaleContainer.SetPosition(new Vector2(0f, 300));
		_scaleContainer.TweenPositionY(0f, 0.4f).SetEasing(Easing.OutSine).Play();

		SetModulate(Colors.Transparent);
		this.TweenModulateAlpha(1f, 0.3f).Play();
		// _scaleContainer.SetScale(Vector2.Zero);
		// _scaleContainer.TweenScale(1f, 1f).SetEasing(Easing.OutBack).Play();

		if(!string.IsNullOrEmpty(_sfxPath))
		{
			AppController.Instance.AudioController.Play(_sfxPath, volumeDb: _sfxVolumeDb, delay: _sfxDelay);
		}

		GTweenSequenceBuilder.New()
			.AppendTime(0.2f + _animationDelay)
			.AppendCallback(() => _animationPlayer.Play("ConditionAnimation"))
			.AppendTime(1.2f)
			.Append(_scaleContainer.TweenScale(0f, 0.4f).SetEasing(Easing.InBack))
			.Build().Play();
	}
}