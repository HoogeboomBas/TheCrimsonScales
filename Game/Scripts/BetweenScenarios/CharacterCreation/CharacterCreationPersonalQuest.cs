using System;
using Godot;
using GTweensGodot.Extensions;

public partial class CharacterCreationPersonalQuest : Control
{
	[Export]
	private RotatingCardView _rotatingCardView;
	[Export]
	private PersonalQuestToggleButton _personalQuestToggleButton;
	[Export]
	private BetterButton _betterButton;

	public PersonalQuestModel PersonalQuestModel => _personalQuestToggleButton.PersonalQuestModel;

	public bool Animating { get; private set; }

	public event Action<CharacterCreationPersonalQuest> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_betterButton.Pressed += OnPressed;
	}

	public void Init(PersonalQuestModel questModel, float rotationDelay)
	{
		_betterButton.SetEnabled(false, false);
		_personalQuestToggleButton.SetModulate(Colors.Transparent);

		Animating = true;

		_rotatingCardView.GetRotationTween(() =>
		{
			_personalQuestToggleButton.Init(questModel);
			_betterButton.SetEnabled(true, false);

			Animating = false;
		}, rotationDelay).Play();
	}

	public void Fade(float target, float duration)
	{
		_personalQuestToggleButton.TweenModulateAlpha(target, duration).Play();
	}

	public void SetSelected(bool active, bool canPress)
	{
		_personalQuestToggleButton.SetSelected(active, canPress);
		_betterButton.SetEnabled(canPress, false);
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}