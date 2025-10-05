using System;
using System.Collections.Generic;
using System.Linq;

public class CardSelectionPrompt(
	Action<List<AbilityCard>> getAllCards,
	CardState? requiredCardState,
	int minSelectionCount,
	int maxSelectionCount,
	EffectCollection effectCollection,
	Func<string> getHintText)
	: Prompt<CardSelectionPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public List<int> CardReferenceIds { get; init; }
	}

	private readonly List<AbilityCard> _cards = new List<AbilityCard>();
	private readonly List<AbilityCard> _selectableCards = new List<AbilityCard>();

	private readonly List<AbilityCard> _selectedCards = new List<AbilityCard>();

	protected override bool CanConfirm =>
		_selectedCards.Count > 0 &&
		(_selectedCards.Count >= minSelectionCount || _selectedCards.Count == _selectableCards.Count) &&
		_selectedCards.Count <= maxSelectionCount;

	protected override bool CanSkip => minSelectionCount == 0 || _selectableCards.Count == 0;

	protected override void Enable()
	{
		base.Enable();

		_cards.Clear();
		getAllCards(_cards);

		_selectableCards.Clear();
		foreach(AbilityCard card in _cards)
		{
			if(!requiredCardState.HasValue || card.CardState == requiredCardState)
			{
				_selectableCards.Add(card);
			}
		}

		_selectedCards.Clear();

		GameController.Instance.CardSelectionView.Open(_cards, OnCardPressed, OnInitiativePressed, null, null);
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		foreach(CardSelectionCard cardSelectionCard in GameController.Instance.CardSelectionView.Cards)
		{
			AbilityCard abilityCard = GameController.Instance.CardManager.Get(cardSelectionCard.SavedAbilityCard);
			bool selected = _selectedCards.Contains(abilityCard);
			cardSelectionCard.SetSelected(selected);
			cardSelectionCard.SetInitiativeSelected(selected);
		}
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.CardSelectionView.Close();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer()
		{
			CardReferenceIds = _selectedCards.Select(card => card.ReferenceId).ToList()
		};
	}

	private void OnCardPressed(CardSelectionCard card)
	{
		AbilityCard abilityCard = GameController.Instance.CardManager.Get(card.SavedAbilityCard);
		if(_selectedCards.Contains(abilityCard))
		{
			_selectedCards.Remove(abilityCard);
		}
		else
		{
			if(!requiredCardState.HasValue || abilityCard.CardState == requiredCardState)
			{
				if(maxSelectionCount == 1)
				{
					_selectedCards.Clear();
				}

				if(_selectedCards.Count < maxSelectionCount)
				{
					_selectedCards.Add(abilityCard);
				}
			}
		}

		FullUpdateState();
	}

	private void OnInitiativePressed(CardSelectionCard card)
	{
		OnCardPressed(card);
	}
}