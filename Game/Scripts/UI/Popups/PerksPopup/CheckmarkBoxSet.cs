using Godot;

public partial class CheckmarkBoxSet : Control
{
	[Export]
	private Control[] _checkmarks;

	private int _checkmarkStartIndex;

	public void Init(int checkmarkStartIndex)
	{
		_checkmarkStartIndex = checkmarkStartIndex;
	}

	public void UpdateCheckmarks(SavedCharacter savedCharacter)
	{
		for(int i = 0; i < _checkmarks.Length; i++)
		{
			Control checkmark = _checkmarks[i];
			checkmark.SetVisible(savedCharacter.CheckmarkCount > _checkmarkStartIndex + i);
		}
	}
}