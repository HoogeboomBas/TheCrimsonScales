using System;
using Godot;
using GTweens.Easings;
using GTweens.Tweens;

public partial class HexPin : Node2D
{
	[Export]
	private Node2D _scaleContainer;
	[Export]
	private WorldButton _worldButton;

	private GTween _scaleTween;

	public bool Active { get; private set; }

	public event Action PressedEvent;
	public event Action<bool> ActiveChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		_scaleContainer.SetScale(Vector2.Zero);

		_worldButton.PressedEvent += OnPressed;
	}

	public void SetHex(Hex hex)
	{
		SetGlobalPosition(hex.GlobalPosition);
	}

	public void SetActive(bool active)
	{
		if(active == Active)
		{
			return;
		}

		Active = active;

		_scaleTween?.Kill();
		if(active)
		{
			_scaleTween = _scaleContainer.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).PlayFastForwardable();
		}
		else
		{
			_scaleTween = _scaleContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).PlayFastForwardable();
		}

		ActiveChangedEvent?.Invoke(Active);
	}

	public void PulsePin()
	{
		_scaleTween?.Complete();
		_scaleTween = _scaleContainer.TweenPulse(1.3f, 0.25f).SetEasing(Easing.OutBack).PlayFastForwardable();
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke();
	}
}