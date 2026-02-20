using System;
using Godot;

public partial class CharacterCreationClass : Control
{
	[Export]
	private ClassToggleButton _classToggleButton;

	public ClassModel ClassModel => _classToggleButton.ClassModel;

	public event Action<CharacterCreationClass> PressedEvent;

	public void Init(ClassModel classModel)
	{
		_classToggleButton.Init(classModel);

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