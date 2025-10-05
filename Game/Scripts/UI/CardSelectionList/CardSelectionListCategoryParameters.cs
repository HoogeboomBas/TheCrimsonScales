using System;
using System.Collections.Generic;

public class CardSelectionListCategoryParameters
{
	public List<SavedAbilityCard> Cards { get; }
	public CardSelectionListCategoryType Type { get; }
	public string HeaderLabel { get; }
	public string HeaderIconPath { get; }

	public Action<CardSelectionCard> CardPressedEvent { get; }
	public Action<CardSelectionCard> InitiativePressedEvent { get; }

	public bool HasHeader => !string.IsNullOrEmpty(HeaderLabel);

	public CardSelectionListCategoryParameters(List<SavedAbilityCard> cards, CardSelectionListCategoryType type,
		Action<CardSelectionCard> cardPressedEvent, Action<CardSelectionCard> initiativePressedEvent)
	{
		Cards = cards;
		Type = type;
		CardPressedEvent = cardPressedEvent;
		InitiativePressedEvent = initiativePressedEvent;

		switch(Type)
		{
			case CardSelectionListCategoryType.None:
				HeaderLabel = null;
				HeaderIconPath = null;
				break;

			case CardSelectionListCategoryType.Active:
				HeaderLabel = "Active";
				HeaderIconPath = Icons.Active;
				break;
			case CardSelectionListCategoryType.Playing:
				HeaderLabel = "Playing";
				HeaderIconPath = Icons.PlayingCards;
				break;
			case CardSelectionListCategoryType.Hand:
				HeaderLabel = "Hand";
				HeaderIconPath = Icons.Cards;
				break;
			case CardSelectionListCategoryType.Discarded:
				HeaderLabel = "Discarded";
				HeaderIconPath = Icons.DiscardedCards;
				break;
			case CardSelectionListCategoryType.Lost:
				HeaderLabel = "Lost";
				HeaderIconPath = Icons.LoseCard;
				break;

			case CardSelectionListCategoryType.Unlockable:
				HeaderLabel = "Unlockable";
				HeaderIconPath = Icons.UnlockableCards;
				break;
			case CardSelectionListCategoryType.Unavailable:
				HeaderLabel = "Unavailable";
				HeaderIconPath = Icons.UnavailableCards;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
		}
	}
}