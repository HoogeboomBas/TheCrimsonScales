using Godot;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public abstract partial class HexObjectEffectViewBase : Node2D
{
	private Node2D _container;

	private bool _hasMoved;
	private GTween _tween;

	public virtual void Init(HexObjectEffectViewParameters parameters)
	{
		_container = GetNode<Node2D>("Container");

		_container.SetScale(Vector2.One * 0.01f);
		_container.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).Play();
	}

	public void Destroy()
	{
		_container.TweenScale(0.5f, 0.2f).SetEasing(Easing.InBack).OnComplete(QueueFree).Play();
	}

	public void Move(Vector2 position)
	{
		_tween?.Kill();

		_tween = this.TweenPosition(position, 0.2f).SetEasing(Easing.OutBack).PlayFastForwardable();

		if(!_hasMoved)
		{
			_tween.Complete();
			_hasMoved = true;
		}
	}

	public void Flash()
	{
		_container.TweenPulse(1.4f, 0.2f).PlayFastForwardable();
	}
}