using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LevelUpCardSelectionPopup : Popup<LevelUpCardSelectionPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private CardSelectionList _availableCardList;
	[Export]
	private CardSelectionList _unlockableCardList;

	protected override void OnOpen()
	{
		base.OnOpen();

		List<SavedAbilityCard> availableCards = PopupRequest.SavedCharacter.AvailableAbilityCards.ToList();
		_availableCardList.Open(availableCards, OnAvailableCardPressed, null,
			(cardA, cardB) => cardA.Model.Initiative.CompareTo(cardB.Model.Initiative));

		List<SavedAbilityCard> unlockableCards =
			PopupRequest.SavedCharacter.ClassModel.AbilityCards
				.Where(card => availableCards.All(availableCard => availableCard.Model != card) && card.Level <= PopupRequest.SavedCharacter.Level)
				.Select(card => new SavedAbilityCard(card)).ToList();

		List<SavedAbilityCard> unavailableCards =
			PopupRequest.SavedCharacter.ClassModel.AbilityCards
				.Where(card => availableCards.All(availableCard => availableCard.Model != card) && card.Level > PopupRequest.SavedCharacter.Level)
				.Select(card => new SavedAbilityCard(card)).ToList();

		_unlockableCardList.Open(
			[
				new CardSelectionListCategoryParameters(unlockableCards, CardSelectionListCategoryType.Unlockable, OnUnlockableCardPressed, null),
				new CardSelectionListCategoryParameters(unavailableCards, CardSelectionListCategoryType.Unavailable, null, null)
			],
			(cardA, cardB) => GetUnlockableCardScore(cardA).CompareTo(GetUnlockableCardScore(cardB)));
	}

	private void OnAvailableCardPressed(CardSelectionCard card)
	{
	}

	private void OnUnlockableCardPressed(CardSelectionCard card)
	{
		_availableCardList.AddCard(card.SavedAbilityCard);

		PopupRequest.SavedCharacter.AddLevelUpCard(card.SavedAbilityCard.Model);

		AppController.Instance.SaveFile.Save();

		Close();
	}

	private static int GetUnlockableCardScore(SavedAbilityCard card)
	{
		int score = 0;

		score += card.Model.Level * 1000;
		score += card.Model.Initiative * 1;

		return score;
	}
}