using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;

public partial class RetaliateEffect : Node2D
{
	[Export]
	private Node2D _rotationContainer;

	private Figure _figure;

	private bool _shakeFigure;
	private Vector2 _figurePosition;
	private int _zIndex;

	private GTween _tweenA;
	private GTween _tweenB;

	public void Init(Figure figure)
	{
		_figure = figure;
		_figurePosition = _figure.Position;
		_zIndex = _figure.ZIndex;

		_rotationContainer.SetModulate(_figure.OutlineColor);

		_figure.SetZIndex(_zIndex + 100);

		_tweenA = GTweenSequenceBuilder.New()
			.AppendCallback(() => _shakeFigure = true)
			.Append(_figure.TweenScale(0.95f, 0.15f).SetEasing(Easing.OutQuad))
			.AppendTime(0.2f)
			.AppendCallback(() =>
			{
				_shakeFigure = false;
				_figure.SetPosition(_figurePosition);
			})
			.Append(_figure.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack))
			.Build().PlayFastForwardable();

		_tweenB = GTweenSequenceBuilder.New()
			.AppendTime(0.4f)
			.Append(this.TweenScale(1.6f, 0.3f).SetEasing(Easing.OutQuad))
			.Append(this.TweenScale(1.5f, 0.15f).SetEasing(Easing.InQuad))
			.AppendTime(0.5f)
			.Append(this.TweenScale(1f, 0.35f).SetEasing(Easing.InBack))
			.AppendCallback(() =>
			{
				_figure.SetZIndex(_zIndex);
			})
			.Build().PlayFastForwardable();

		this.DelayedCall(QueueFree, 3f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_shakeFigure)
		{
			_figure.SetPosition(_figurePosition + new Vector2(GD.Randf() - 0.5f, GD.Randf() - 0.5f) * 8f);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		_tweenA?.Kill();
		_tweenB?.Kill();
	}
}