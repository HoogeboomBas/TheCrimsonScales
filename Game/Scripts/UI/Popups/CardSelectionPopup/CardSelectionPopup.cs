using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CardSelectionPopup : Popup<CardSelectionPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private CardSelectionList _handCardList;
	[Export]
	private CardSelectionList _availableCardList;

	[Export]
	private Label _requiredCountLabel;

	protected override void OnOpen()
	{
		base.OnOpen();

		List<SavedAbilityCard> handCards = PopupRequest.SavedCharacter.HandAbilityCardIndices
			.Select(cardIndex => PopupRequest.SavedCharacter.AvailableAbilityCards[cardIndex]).ToList();
		_handCardList.Open(handCards, OnHandCardPressed, null,
			(cardA, cardB) => cardA.Model.Initiative.CompareTo(cardB.Model.Initiative));

		List<SavedAbilityCard> availableCards = PopupRequest.SavedCharacter.AvailableAbilityCards.Where(card => !handCards.Contains(card)).ToList();
		_availableCardList.Open(availableCards, OnAvailableCardPressed, null,
			(cardA, cardB) => cardA.Model.Initiative.CompareTo(cardB.Model.Initiative));

		UpdateLabel();
	}

	protected override void OnClosed()
	{
		PopupRequest.SavedCharacter.HandAbilityCardIndices.Clear();
		foreach(CardSelectionCard card in _handCardList.Cards)
		{
			PopupRequest.SavedCharacter.HandAbilityCardIndices.Add(PopupRequest.SavedCharacter.AvailableAbilityCards.IndexOf(card.SavedAbilityCard));
		}

		base.OnClosed();

		AppController.Instance.SaveFile.Save();
	}

	private void UpdateLabel()
	{
		int currentHandSize = _handCardList.Cards.Count();
		int requiredHandSize = PopupRequest.SavedCharacter.ClassModel.HandSize;
		_requiredCountLabel.SetText($"{currentHandSize}/{requiredHandSize}");

		bool correctHandSize = currentHandSize == requiredHandSize;
		_requiredCountLabel.SetModulate(correctHandSize ? Colors.White : Color.FromHtml("ff5e3a"));

		SetCanClose(correctHandSize);
	}

	private void OnHandCardPressed(CardSelectionCard card)
	{
		_handCardList.RemoveCard(card.SavedAbilityCard);
		_availableCardList.AddCard(card.SavedAbilityCard);

		UpdateLabel();
	}

	private void OnAvailableCardPressed(CardSelectionCard card)
	{
		_availableCardList.RemoveCard(card.SavedAbilityCard);
		_handCardList.AddCard(card.SavedAbilityCard);

		UpdateLabel();
	}
}