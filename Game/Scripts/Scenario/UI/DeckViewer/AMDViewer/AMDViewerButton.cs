using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class AMDViewerButton : DeckViewerButton<AMDCard>
{
	[Export]
	public RichTextLabel ExtraDetailLabel;

	public override bool CardCountAvailable(CardCount cardCount, AMDCard card)
	{
		return cardCount.Card.Model.ToString(new RichTextParameters()) == card.Model.ToString(new RichTextParameters());
	}

	public override List<CardCount> SortCardCounts(List<CardCount> cardCounts)
	{
		return cardCounts
			.OrderBy(count => count.Card.Model.Type switch
			{
				AMDCardType.Null => 0,
				AMDCardType.Value => 1,
				AMDCardType.Crit => 2,
				_ => 3
			})
			.ThenBy(count => count.Card.Model.GetValue(null) ?? 0)
			.ThenBy(count => count.Card.Model.GetRolling(null))
			.ThenBy(count => count.Card.Model.ToString())
			.ToList();
	}

	public override void CreateCards(List<CardCount> sortedCards)
	{
		foreach(CardCount cardCount in sortedCards)
		{
			AMDViewerBox box = DeckViewerBox.Instantiate<AMDViewerBox>();
			box.SetDeckViewerButton(this);
			box.SetCard(cardCount.Card, cardCount.DeckCount, cardCount.DiscardCount);
			Grid.AddChild(box);
		}
	}
}