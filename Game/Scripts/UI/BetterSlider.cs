using System;
using Godot;

public partial class BetterSlider : Control
{
	private static readonly Color ButtonUpColor = Colors.White;
	private static readonly Color ButtonDownColor = new Color(0.9f, 0.9f, 0.9f);

	[Export]
	private Slider _slider;
	[Export]
	private TextureRect _grabber;

	private DateTime _buttonDownTimeStamp;

	public float Value => (float)_slider.Value;

	public event Action<float> ValueChangedEvent;

	public override void _EnterTree()
	{
		base._EnterTree();

		_slider.ValueChanged += OnValueChanged;
		_slider.DragStarted += OnDragStarted;
		_slider.DragEnded += OnDragEnded;
	}

	public override void _Ready()
	{
		base._Ready();

		OnValueChanged(_slider.Value);
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		_slider.ValueChanged -= OnValueChanged;
	}

	public void SetValue(float value)
	{
		if(_slider.Value == value)
		{
			// Make sure the value change is forced, so the handle position is updated appropriately
			OnValueChanged(_slider.Value);
		}
		else
		{
			_slider.SetValue(value);
		}
	}

	public void SetRange(float min, float max)
	{
		_slider.SetMin(min);
		_slider.SetMax(max);
	}

	private void OnValueChanged(double value)
	{
		float normalizedValue = Mathf.InverseLerp((float)_slider.MinValue, (float)_slider.MaxValue, (float)_slider.Value);
		_grabber.Position = new Vector2(normalizedValue * Size.X - _grabber.Size.X / 2, _grabber.Position.Y);
		ValueChangedEvent?.Invoke(Value);
	}

	private void OnDragStarted()
	{
		_grabber.PivotOffset = _grabber.Size * 0.5f;
		_grabber.Scale = 0.95f * Vector2.One;
		Modulate = ButtonDownColor;

		_buttonDownTimeStamp = DateTime.Now;
		AppController.Instance.AudioController.Play(SFX.Click, 3f, 3.2f, volumeDb: -8);
		VibrationController.Vibrate();
	}

	private void OnDragEnded(bool valueChanged)
	{
		_grabber.PivotOffset = _grabber.Size * 0.5f;
		_grabber.Scale = Vector2.One;
		Modulate = ButtonUpColor;

		if((DateTime.Now - _buttonDownTimeStamp).TotalSeconds > 0.3f)
		{
			_buttonDownTimeStamp = DateTime.Now;
			AppController.Instance.AudioController.Play(SFX.Click, 2.5f, 2.7f, volumeDb: -8);
			VibrationController.Vibrate();
		}
	}
}