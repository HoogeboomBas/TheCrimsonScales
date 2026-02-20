using System;
using Godot;

public partial class ItemRewardCharacterSelectionPopupCharacter : Control
{
	[Export]
	private ClassToggleButton _classToggleButton;

	public SavedCharacter SavedCharacter { get; private set; }

	public event Action<ItemRewardCharacterSelectionPopupCharacter> PressedEvent;

	public void Init(SavedCharacter savedCharacter)
	{
		SavedCharacter = savedCharacter;

		_classToggleButton.Init(SavedCharacter.ClassModel);

		_classToggleButton.PressedEvent += OnPressed;
	}

	public void SetSelected(bool active, bool canPress)
	{
		_classToggleButton.SetSelected(active, canPress);
	}

	private void OnPressed(ClassToggleButton classToggleButton)
	{
		PressedEvent?.Invoke(this);
	}
}