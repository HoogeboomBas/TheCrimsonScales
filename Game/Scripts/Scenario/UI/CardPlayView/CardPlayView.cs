using System;
using System.Collections.Generic;
using Godot;

public partial class CardPlayView : Control
{
	[Export]
	private CardPlayCard[] _cards;

	public event Action<CardPlayCard, AbilityCardSection> CardSectionPressed;

	public override void _Ready()
	{
		base._Ready();

		foreach(CardPlayCard card in _cards)
		{
			card.CardSectionPressedEvent += OnCardSectionPressed;
		}

		Hide();
	}

	public void Open(List<CardPlayCardData> cardDatas, Action<CardPlayCard, AbilityCardSection> cardSectionPressed)
	{
		if(cardDatas.Count > 2)
		{
			Log.Error("Trying to open card play view with more than 2 cards.");
			return;
		}

		Show();

		CardSectionPressed = cardSectionPressed;

		_cards[0].SetCardData(cardDatas[0]);
		_cards[1].SetCardData(cardDatas.Count > 1 ? cardDatas[1] : null);
	}

	public void Close()
	{
		Hide();

		foreach(CardPlayCard card in _cards)
		{
			card.Close();
		}
	}

	private void OnCardSectionPressed(CardPlayCard card, AbilityCardSection section)
	{
		CardSectionPressed?.Invoke(card, section);
	}
}