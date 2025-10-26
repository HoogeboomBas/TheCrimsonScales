using System;
using Godot;

[GlobalClass]
public partial class BetterButton : BaseButton
{
	private static readonly Color ButtonUpColor = Colors.White;
	private static readonly Color ButtonDownColor = new Color(0.9f, 0.9f, 0.9f);
	private static readonly Color DisabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Export]
	private bool _centerPivot = true;

	private bool _active;
	private bool _hovered;
	private bool _down;
	private bool _canChangeColor = true;

	private DateTime _buttonDownTimeStamp;

	public override void _Ready()
	{
		base._Ready();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		ButtonDown += OnButtonDown;
		ButtonUp += OnButtonUp;

		if(_centerPivot)
		{
			this.DelayedCall(CenterPivot);
		}

		UpdateVisuals();
		//MouseDefaultCursorShape = CursorShape.PointingHand;
	}

	public void SetEnabled(bool enabled, bool canChangeColor = true)
	{
		_canChangeColor = canChangeColor;
		Disabled = !enabled;
		UpdateVisuals();
	}

	private void OnMouseEntered()
	{
		_hovered = true;
		UpdateVisuals();
	}

	private void OnMouseExited()
	{
		_hovered = false;
		UpdateVisuals();
	}

	private void OnButtonDown()
	{
		if(Disabled)
		{
			UpdateVisuals();
			return;
		}

		_down = true;
		UpdateVisuals();

		_buttonDownTimeStamp = DateTime.Now;
		AppController.Instance.AudioController.Play(SFX.Click, 3f, 3.2f, volumeDb: -8);
		VibrationController.Vibrate();
	}

	private void OnButtonUp()
	{
		_down = false;

		if(Disabled)
		{
			UpdateVisuals();
			return;
		}

		UpdateVisuals();

		if((DateTime.Now - _buttonDownTimeStamp).TotalSeconds > 0.3f)
		{
			_buttonDownTimeStamp = DateTime.Now;
			AppController.Instance.AudioController.Play(SFX.Click, 2.5f, 2.7f, volumeDb: -8);
			VibrationController.Vibrate();
		}
	}

	private void CenterPivot()
	{
		PivotOffset = Size * 0.5f;
	}

	private void UpdateVisuals()
	{
		if(_centerPivot)
		{
			CenterPivot();
		}

		float scale = 1f;
		if(!Disabled)
		{
			if(_down)
			{
				scale = 0.98f;
			}
			else if(_hovered)
			{
				scale = 1.03f;
			}
		}

		if(_canChangeColor)
		{
			Modulate = Disabled ? DisabledColor : _down ? ButtonDownColor : ButtonUpColor;
		}

		Scale = scale * Vector2.One;
	}
}