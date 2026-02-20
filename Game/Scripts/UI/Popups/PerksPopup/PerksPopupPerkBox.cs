using Godot;

public partial class PerksPopupPerkBox : Control
{
	[Export]
	private Control _checkmark;

	public void SetAcquired(bool acquired)
	{
		_checkmark.SetVisible(acquired);
	}
}