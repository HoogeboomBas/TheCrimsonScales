using Godot;

public partial class CharacterNameCharacterCreationStep : CharacterCreationStep
{
	[Export]
	private LineEdit _nameLineEdit;

	public override bool ConfirmButtonActive => !string.IsNullOrEmpty(_nameLineEdit.Text);

	public override void _Ready()
	{
		base._Ready();

		_nameLineEdit.TextChanged += OnNameChanged;

		OnNameChanged(_nameLineEdit.Text);
	}

	public override void Activate()
	{
		base.Activate();

		_nameLineEdit.SetText(_characterCreationOverlay.CharacterName);
		OnNameChanged(_nameLineEdit.Text);
	}

	private void OnNameChanged(string newText)
	{
		if(Active)
		{
			_characterCreationOverlay.SetCharacterName(newText);
			_characterCreationOverlay.UpdateConfirmVisible();
		}
	}
}