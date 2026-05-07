using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ExclamationMark : Control
{
	private bool _active;

	private GTween _tween;

	public override void _EnterTree()
	{
		base._EnterTree();

		SetVisible(false);
		SetPivotOffset(Size * 0.5f);
	}

	public void SetActive(bool active)
	{
		if(active == _active)
		{
			return;
		}

		_active = active;

		SetVisible(_active);
		SetScale(Vector2.One);

		_tween?.Kill();
		_tween = GTweenSequenceBuilder.New()
			.Append(this.TweenScale(1.2f, 1f).SetEasing(Easing.InOutQuad))
			.AppendTime(0.2f)
			.Append(this.TweenScale(1f, 1f).SetEasing(Easing.InOutQuad))
			.AppendTime(0.2f)
			.Build().SetMaxLoops().Play();
	}
}