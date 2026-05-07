using Godot;

public partial class SaveFileSelectionPopupSaveFileCharacter : Control
{
	[Export]
	private ClassView _classView;

	public void Init(SavedCharacter savedCharacter)
	{
		_classView.Init(savedCharacter.ClassModel);
	}
}