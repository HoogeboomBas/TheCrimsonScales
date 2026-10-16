using Godot;

public partial class PerksPopupMastery : Control
{
	[Export]
	private MasteryBox _masteryBox;
	[Export]
	private RichTextLabel _description;

	public Mastery Mastery { get; private set; }

	public void Init(Mastery mastery)
	{
		Mastery = mastery;

		_description.SetText(mastery.Model.Description);

		_masteryBox.Checkmark.SetVisible(mastery.Completed);
	}
}