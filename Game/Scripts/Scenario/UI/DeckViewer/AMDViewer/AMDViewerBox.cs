using Godot;

public partial class AMDViewerBox : DeckViewerBox<AMDCard, AMDViewerButton>
{
	[Export]
	private RichTextLabel _label;

	public override void SetCard(AMDCard card, int deckCount, int discardCount)
	{
		base.SetCard(card, deckCount, discardCount);
		_label.Text = card.Model.GetSimpleString(_label.GetRichTextParameters());
	}

	protected override void OnMouseEntered()
	{
		base.OnMouseEntered();
		DeckViewerButton.ExtraDetailLabel.SetText(Card.Model.ToString(DeckViewerButton.ExtraDetailLabel.GetRichTextParameters()));
	}
}