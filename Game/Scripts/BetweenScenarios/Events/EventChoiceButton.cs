using System;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class EventChoiceButton : Control
{
	[Export]
	private Control _scaleContainer;
	[Export]
	private BetterButton _button;
	[Export]
	private RichTextLabel _richTextLabel;

	public EventChoiceModel Model { get; private set; }

	private event Action<EventChoiceButton> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_scaleContainer.SetPivotOffset(Size * 0.5f);
		_scaleContainer.SetScale(0.001f * Vector2.One);

		_button.Pressed += OnPressed;
	}

	public void Init(EventChoiceModel model, Action<EventChoiceButton> onPressed)
	{
		Model = model;
		PressedEvent = onPressed;

		_richTextLabel.SetText(Model.ChoiceText);
	}

	public void SetActive(bool active)
	{
		if(active)
		{
			Show();
			_scaleContainer.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).Play();
		}
		else
		{
			_scaleContainer.TweenScale(0f, 0.3f).SetEasing(Easing.InBack).OnComplete(Hide).Play();
		}
	}

	public void Disable()
	{
		_button.SetEnabled(false, false);
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}