using Godot;

public partial class MonsterDeckViewerBox : DeckViewerBox<MonsterAbilityCard, MonsterDeckViewerButton>
{
	[Export]
	private TextureRect _textureRect;

	public override void SetCard(MonsterAbilityCard card, int deckCount, int discardCount)
	{
		base.SetCard(card, deckCount, discardCount);
		_textureRect.SetTexture(card.GetTexture());
	}

	protected override void OnMouseEntered()
	{
		base.OnMouseEntered();
		DeckViewerButton.ExtraDetailTexture.SetTexture(Card.GetTexture());
	}
}