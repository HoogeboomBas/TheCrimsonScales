using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class CardSelectionPhase : ScenarioPhase
{
	private CardSelectionState _cardSelectionState;

	private Character _selectedCharacter;

	private int _syncedActionIndex;

	private bool FirstRound => GameController.Instance.ScenarioPhaseManager.RoundIndex == 0;

	public override async GDTask Activate()
	{
		await base.Activate();

		// Start of round before card selection
		await ScenarioEvents.RoundStartBeforeCardSelectionEvent.CreatePrompt(
			new ScenarioEvents.RoundStartBeforeCardSelection.Parameters(), GameController.Instance.CharacterManager.GetCharacter(0));

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			character.RoundCards.Clear();
			character.SetLongResting(false);
			character.SetShortRestSeed(GameController.Instance.StateRNG.RandiRange(0, int.MaxValue));

			// Exhaust if this character does not have enough cards left
			int playableCardCount = character.Cards.Count(card => card.CardState == CardState.Hand);
			int discardedCardCount = character.Cards.Count(card => card.CardState == CardState.Discarded);

			if(playableCardCount < 2 && discardedCardCount < 2)
			{
				await AbilityCmd.KillOrExhaust(null, character);
			}
		}

		if(GameController.Instance.ScenarioPhaseManager.RoundIndex < GameController.Instance.SavedCampaign.SavedScenario.CardSelectionStates.Count)
		{
			// Save data contains data for this card selection phase
			//GameController.SetFastForward(true);

			_cardSelectionState =
				GameController.Instance.SavedCampaign.SavedScenario.CardSelectionStates[GameController.Instance.ScenarioPhaseManager.RoundIndex];

			//SetState(fullState);
		}
		else
		{
			_cardSelectionState = CreateInitialState();
			GameController.Instance.SavedCampaign.SavedScenario.CardSelectionStates.Add(_cardSelectionState);
		}

		if(!_cardSelectionState.Completed)
		{
			GameController.SetFastForward(false);

			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				character.CardStateChangedEvent += OnCardStateChanged;
				character.CardAddedEvent += OnCardAdded;
				character.CardRemovedEvent += OnCardRemoved;
				character.DestroyedEvent += OnDestroyed;
			}

			// Set selected character to the first in order that isn't exhausted
			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				if(!character.IsDead)
				{
					SetSelectedCharacter(character);
					break;
				}
			}

			foreach(PortraitViewCharacterPortrait portrait in GameController.Instance.PortraitView.CharacterPortraits)
			{
				portrait.PressedEvent += OnPortraitPressed;
			}

			//GameController.Instance.ShortRestView.ConfirmedEvent += OnShortRestConfirmed;

			GameController.Instance.CardSelectionButtonsView.Open(OnContinuePressed);
			GameController.Instance.CardSelectionButtonsView.SetButtons(true);
			GameController.Instance.UndoView.Open(this);
			GameController.Instance.PromptManager.PromptStartedEvent += OnPromptStarted;
			GameController.Instance.PromptManager.PromptEndedEvent += OnPromptEnded;
		}

		//SetState(_cardSelectionState);
		SyncGameWithState();

		await PerformSyncedActions();

		while(!_cardSelectionState.Completed)
		{
			// Wait until a new synced action has become available, or the card selection phase is completed
			await GDTask.WaitUntil(
				() =>
					_cardSelectionState.Completed ||
					_syncedActionIndex < _cardSelectionState.SyncedActions.Count ||
					GameController.Instance.ResignRequested ||
					GameController.Instance.CheatWinRequested,
				cancellationToken: GameController.CancellationToken);

			await GameController.Instance.CheckEarlyEnd();

			await PerformSyncedActions();
		}

		// Final state update
		//SetState(_cardSelectionState);
		SyncGameWithState();

		// foreach(Character character in GameController.Instance.CharacterManager.Characters)
		// {
		// 	character.RoundCardsChangedEvent -= OnRoundCardsChanged;
		// }

		foreach(PortraitViewPortrait portrait in GameController.Instance.PortraitView.Portraits)
		{
			portrait.SetSelected(false);
			portrait.PressedEvent -= OnPortraitPressed;
		}

		GameController.Instance.CardSelectionButtonsView.Close();
		GameController.Instance.UndoView.Close(this);
		GameController.Instance.PromptManager.PromptStartedEvent -= OnPromptStarted;
		GameController.Instance.PromptManager.PromptEndedEvent -= OnPromptEnded;

		GameController.Instance.CardSelectionView.Close();
		GameController.Instance.HexIndicatorManager.ClearIndicators();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			character.CardStateChangedEvent -= OnCardStateChanged;
			character.CardAddedEvent -= OnCardAdded;
			character.CardRemovedEvent -= OnCardRemoved;
			character.DestroyedEvent -= OnDestroyed;

			foreach(AbilityCard card in character.RoundCards)
			{
				await card.SetCardState(CardState.Playing);
			}
		}
	}

	private void OnPromptStarted(Character character)
	{
		if(character.IsLocal)
		{
			GameController.Instance.CardSelectionButtonsView.Block(this);
		}
	}

	private void OnPromptEnded(Character character)
	{
		if(character.IsLocal)
		{
			GameController.Instance.CardSelectionButtonsView.UnBlock(this);
		}
	}

	private CardSelectionState CreateInitialState()
	{
		CardSelectionState fullState = new CardSelectionState()
		{
			CharacterCardSelectionStates = new CharacterCardSelectionState[GameController.Instance.CharacterManager.Characters.Count],
			CurrentPromptIndex = GameController.Instance.SavedScenario.PromptAnswers.Count,
			SyncedActions = new List<SyncedAction>()
		};

		for(int i = 0; i < fullState.CharacterCardSelectionStates.Length; i++)
		{
			fullState.CharacterCardSelectionStates[i] = new CharacterCardSelectionState
			{
				ChosenCardReferenceIds = new List<int>(),
				LongResting = false
			};
		}

		return fullState;
	}

	private void SyncGameWithState()
	{
		for(int i = 0; i < _cardSelectionState.CharacterCardSelectionStates.Length; i++)
		{
			Character character = GameController.Instance.CharacterManager.GetCharacter(i);
			SyncCharacterWithState(character);
		}
	}

	private void SyncCharacterWithState(Character character)
	{
		CharacterCardSelectionState state = _cardSelectionState.CharacterCardSelectionStates[character.Index];
		character.SetLongResting(state.LongResting);
		character.RoundCards.Clear();
		character.RoundCards.AddRange(state.ChosenCardReferenceIds.Select(referenceId =>
			GameController.Instance.ReferenceManager.Get<AbilityCard>(referenceId)));

		character.OnRoundCardsChanged();

		if(character == _selectedCharacter)
		{
			UpdateCardSelectionView();
		}

		UpdateChoiceButtons();
	}

	private void SetSelectedCharacter(Character character)
	{
		_selectedCharacter = character;
		GameController.Instance.CardSelectionView.Open(_selectedCharacter.Cards, OnCardPressed, OnInitiativePressed, OnShortRestPressed,
			OnLongRestPressed);
		UpdateCardSelectionView();
		UpdateHexIndicators();

		foreach(PortraitViewPortrait portrait in GameController.Instance.PortraitView.Portraits)
		{
			portrait.SetSelected(portrait is PortraitViewCharacterPortrait characterPortrait && characterPortrait.Character == character);
		}
	}

	private void UpdateHexIndicators()
	{
		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			if(character.IsDead)
			{
				continue;
			}

			GameController.Instance.HexIndicatorManager.SetIndicator(
				character.Hex, _selectedCharacter == character ? HexIndicatorType.Selected : HexIndicatorType.Normal, OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();
	}

	private void UpdateCardSelectionView()
	{
		foreach(CardSelectionCard cardSelectionCard in GameController.Instance.CardSelectionView.Cards)
		{
			bool selected = false;

			for(int i = 0; i < _selectedCharacter.RoundCards.Count; i++)
			{
				AbilityCard card = _selectedCharacter.RoundCards[i];
				AbilityCard abilityCard = GameController.Instance.CardManager.Get(cardSelectionCard.SavedAbilityCard);
				if(abilityCard == card)
				{
					selected = true;
					cardSelectionCard.SetSelected(true);
					cardSelectionCard.SetInitiativeSelected(i == 0);
					break;
				}
			}

			if(!selected)
			{
				cardSelectionCard.SetSelected(false);
				cardSelectionCard.SetInitiativeSelected(false);
			}
		}

		GameController.Instance.CardSelectionView.SetLongRestSelected(_selectedCharacter.LongResting);

		GameController.Instance.CardSelectionView.SetRestingEnabled(
			_selectedCharacter.Cards.Count(card => card.CardState == CardState.Discarded) >= 2);
	}

	private bool TryAddSyncedAction(SyncedAction syncedAction)
	{
		if(_syncedActionIndex < _cardSelectionState.SyncedActions.Count)
		{
			// Still performing a synced action
			return false;
		}

		//TODO: Sync with server
		_cardSelectionState.SyncedActions.Add(syncedAction);
		//SetState(_cardSelectionState);

		return true;
	}

	private async GDTask PerformSyncedActions()
	{
		while(_syncedActionIndex < _cardSelectionState.SyncedActions.Count)
		{
			// Perform all the synced actions
			await _cardSelectionState.SyncedActions[_syncedActionIndex].Perform();
			_syncedActionIndex++;

			//SetState(_cardSelectionState);
		}

		SyncGameWithState();
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		Character otherCharacter = hexIndicator.Hex.GetHexObjectOfType<Character>();
		if(otherCharacter != null)
		{
			if(otherCharacter == _selectedCharacter)
			{
			}
			else
			{
				SetSelectedCharacter(otherCharacter);
			}
		}
	}

	private void UpdateChoiceButtons()
	{
		bool hasUnfinishedCharacter = false;
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			if(!character.IsLocal || character.IsDestroyed)
			{
				continue;
			}

			if(character.RoundCards.Count < 2 && !character.LongResting)
			{
				hasUnfinishedCharacter = true;
				break;
			}
		}

		GameController.Instance.CardSelectionButtonsView.SetButtons(!hasUnfinishedCharacter);
	}

	private void OnCardStateChanged(Character character, AbilityCard abilityCard)
	{
		if(character == _selectedCharacter)
		{
			SetSelectedCharacter(_selectedCharacter);
		}
	}

	private void OnCardAdded(Character character, AbilityCard abilityCard)
	{
		if(character == _selectedCharacter)
		{
			SetSelectedCharacter(_selectedCharacter);
		}
	}

	private void OnCardRemoved(Character character, AbilityCard abilityCard)
	{
		if(character == _selectedCharacter)
		{
			SetSelectedCharacter(_selectedCharacter);
		}
	}

	private void OnDestroyed(Figure figure)
	{
		if(_selectedCharacter == figure)
		{
			foreach(Character otherCharacter in GameController.Instance.CharacterManager.Characters)
			{
				if(!otherCharacter.IsDead)
				{
					SetSelectedCharacter(otherCharacter);
					break;
				}
			}
		}
	}

	private void OnCardPressed(CardSelectionCard cardSelectionCard)
	{
		if(_cardSelectionState.Completed)
		{
			return;
		}

		AbilityCard card = GameController.Instance.CardManager.Get(cardSelectionCard.SavedAbilityCard);

		if(card.CardState == CardState.Persistent || card.CardState == CardState.PersistentLoss)
		{
			AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Deactivate card",
				$"Are you sure you want to {(card.CardState == CardState.Persistent ? "discard" : "lose")} {card.Model.Name}?",
				new TextButton.Parameters("Cancel", () =>
				{
				}),
				new TextButton.Parameters("Confirm", () =>
				{
					TryAddSyncedAction(new DeactivateActiveCardSyncedAction(card.Owner, card));
				}, TextButton.ColorType.Red)
			));

			return;
		}

		if(card.CardState != CardState.Hand)
		{
			return;
		}

		CharacterCardSelectionState characterCardSelectionState = _cardSelectionState.CharacterCardSelectionStates[card.Owner.Index];
		if(_selectedCharacter.RoundCards.Contains(card))
		{
			characterCardSelectionState.ChosenCardReferenceIds.Remove(card.ReferenceId);
		}
		else if(_selectedCharacter.RoundCards.Count < _selectedCharacter.PlayableAbilityCardCount)
		{
			characterCardSelectionState.ChosenCardReferenceIds.Add(card.ReferenceId);
		}

		characterCardSelectionState.LongResting = false;

		SyncCharacterWithState(card.Owner);
	}

	private void OnInitiativePressed(CardSelectionCard cardSelectionCard)
	{
		if(_cardSelectionState.Completed)
		{
			return;
		}

		AbilityCard abilityCard = GameController.Instance.CardManager.Get(cardSelectionCard.SavedAbilityCard);

		CharacterCardSelectionState characterCardSelectionState = _cardSelectionState.CharacterCardSelectionStates[abilityCard.Owner.Index];

		int index = characterCardSelectionState.ChosenCardReferenceIds.IndexOf(abilityCard.ReferenceId);
		if(index == 0)
		{
			characterCardSelectionState.ChosenCardReferenceIds.Remove(abilityCard.ReferenceId);
			characterCardSelectionState.ChosenCardReferenceIds.Add(abilityCard.ReferenceId);
		}
		else if(index > 0)
		{
			characterCardSelectionState.ChosenCardReferenceIds.Remove(abilityCard.ReferenceId);
			characterCardSelectionState.ChosenCardReferenceIds.Insert(0, abilityCard.ReferenceId);
		}
		else if(_selectedCharacter.RoundCards.Count < _selectedCharacter.PlayableAbilityCardCount)
		{
			characterCardSelectionState.ChosenCardReferenceIds.Insert(0, abilityCard.ReferenceId);
		}

		characterCardSelectionState.LongResting = false;

		SyncCharacterWithState(abilityCard.Owner);
	}

	private void OnShortRestPressed()
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Short rest",
			$"Are you sure you want to short rest?",
			new TextButton.Parameters("Cancel", () =>
			{
			}),
			new TextButton.Parameters("Confirm", () =>
			{
				CharacterCardSelectionState characterCardSelectionState = GetCharacterCardSelectionState(_selectedCharacter);
				characterCardSelectionState.LongResting = false;
				characterCardSelectionState.ChosenCardReferenceIds.Clear();
				SyncCharacterWithState(_selectedCharacter);

				TryAddSyncedAction(new ShortRestSyncedAction(_selectedCharacter));
			}, TextButton.ColorType.Red)
		));
	}

	private void OnLongRestPressed()
	{
		CharacterCardSelectionState characterCardSelectionState = GetCharacterCardSelectionState(_selectedCharacter);
		characterCardSelectionState.LongResting = true;
		characterCardSelectionState.ChosenCardReferenceIds.Clear();

		SyncCharacterWithState(_selectedCharacter);
	}

	private void OnPortraitPressed(PortraitViewPortrait portrait)
	{
		if(portrait is not PortraitViewCharacterPortrait characterPortrait)
		{
			return;
		}

		Character otherCharacter = characterPortrait.Character;

		if(otherCharacter == _selectedCharacter)
		{
		}
		else
		{
			SetSelectedCharacter(otherCharacter);
		}
	}

	private void OnContinuePressed()
	{
		_cardSelectionState.Completed = true;
	}

	private CharacterCardSelectionState GetCharacterCardSelectionState(Character character)
	{
		return _cardSelectionState.CharacterCardSelectionStates[character.Index];
	}
}