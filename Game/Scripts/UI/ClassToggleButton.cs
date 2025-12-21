using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ClassToggleButton : Control
{
	[Export]
	private BetterButton _button;

	[Export]
	private Control _container;
	[Export]
	private ClassView _classView;
	[Export]
	private Control _inactiveOverlay;

	private bool _selected;
	private GTween _scaleTween;

	public ClassModel ClassModel { get; private set; }

	public event Action<ClassToggleButton> PressedEvent;

	public void Init(ClassModel classModel)
	{
		ClassModel = classModel;

		_classView.Init(ClassModel);

		this.DelayedCall(() =>
		{
			_container.PivotOffset = _container.Size * 0.5f;
		});

		_selected = true;
		_inactiveOverlay.TweenModulateAlpha(0f, 0f).Play();

		_button.SetEnabled(true, false);
		_button.Pressed += OnPressed;
	}

	public void SetSelected(bool active, bool canPress)
	{
		_button.SetEnabled(canPress, false);

		if(_selected == active)
		{
			return;
		}

		_selected = active;

		_scaleTween?.Kill();
		if(_selected)
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(0f, 0.15f))
				.Build().Play();
		}
		else
		{
			_scaleTween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_container.TweenScale(0.9f, 0.15f).SetEasing(Easing.InBack))
				.Join(_inactiveOverlay.TweenModulateAlpha(1f, 0.15f))
				.Build().Play();
		}
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}