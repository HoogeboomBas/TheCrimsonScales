using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class MonsterDeckViewerButton : DeckViewerButton<MonsterAbilityCard>
{
	[Export]
	private TextureRect _monsterDeckBackTexture;

	[Export]
	public TextureRect ExtraDetailTexture;

	public override void SetCardDeck(CardDeck<MonsterAbilityCard> deck)
	{
		base.SetCardDeck(deck);

		_monsterDeckBackTexture.SetTexture(AtlasTextureHelper.CreateAtlasTexture(
			8, 3, 3,
			ResourceLoader.Load<Texture2D>(deck.DrawPile.First().Model.CardsAtlasPath)));
	}

	public override bool CardCountAvailable(CardCount cardCount, MonsterAbilityCard card)
	{
		return cardCount.Card.Model == card.Model;
	}

	public override List<CardCount> SortCardCounts(List<CardCount> cardCounts)
	{
		return cardCounts.OrderBy(count => count.Card.Model.Initiative).ThenBy(count => count.Card.Model.CardIndex).ToList();
	}

	public override void CreateCards(List<CardCount> sortedCards)
	{
		foreach(CardCount cardCount in sortedCards)
		{
			MonsterDeckViewerBox box = DeckViewerBox.Instantiate<MonsterDeckViewerBox>();
			box.SetDeckViewerButton(this);
			box.SetCard(cardCount.Card, cardCount.DeckCount, cardCount.DiscardCount);
			Grid.AddChild(box);
		}
	}
}