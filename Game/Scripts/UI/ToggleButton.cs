using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ToggleButton<T> : Control
	where T : ToggleButton<T>
{
	[Export]
	private BetterButton _button;

	[Export]
	private Control _container;
	[Export]
	private Control _inactiveOverlay;

	private bool _selected;
	private GTween _scaleTween;

	public event Action<T> PressedEvent;

	protected void Init()
	{
		this.DelayedCall(() =>
		{
			_container.PivotOffset = _container.Size * 0.5f;
		});

		_selected = true;
		_inactiveOverlay.TweenModulateAlpha(0f, 0f).Play();

		_button.SetEnabled(true, false);
		_button.Pressed += OnPressed;
	}

	public void SetSelected(bool selected, bool canPress, bool skipAnimation = false)
	{
		_button.SetEnabled(canPress, false);

		if(_selected == selected)
		{
			return;
		}

		_selected = selected;

		_scaleTween?.Kill();
		if(_selected)
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(0f, 0.15f))
				.Build().Play(skipAnimation);
		}
		else
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(0.9f, 0.15f).SetEasing(Easing.InBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(1f, 0.15f))
				.Build().Play(skipAnimation);
		}
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke((T)this);
	}
}