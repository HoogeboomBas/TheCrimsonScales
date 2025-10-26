using System.Collections.Generic;
using Fractural.Tasks;

public class RoundPhase : ScenarioPhase
{
	private readonly List<Figure> _sortedFigures = new List<Figure>();
	private bool _sortingRequired;

	public override async GDTask Activate()
	{
		await base.Activate();

		foreach(MonsterGroup monsterGroup in GameController.Instance.Map.MonsterGroups)
		{
			monsterGroup.TryDrawCard();
		}

		// Start of round
		// Trigger any start-of-round effects in the scenario rules or on items or character ability cards
		await ScenarioEvents.RoundStartedBeforeInitiativesSortedEvent.CreatePrompt(
			new ScenarioEvents.RoundStartedBeforeInitiativesSorted.Parameters(GameController.Instance.ScenarioPhaseManager.RoundIndex),
			GameController.Instance.CharacterManager.GetCharacter(0));

		// Take turns
		GameController.Instance.Map.FigureAddedEvent += OnFigureAdded;
		GameController.Instance.Map.FigureRemovedEvent += OnFigureRemoved;

		UpdateFigures();

		// Trigger any "after sorting of initiatives" effects
		await ScenarioEvents.InitiativesSortedEvent.CreatePrompt(
			new ScenarioEvents.InitiativesSorted.Parameters(GameController.Instance.ScenarioPhaseManager.RoundIndex),
			GameController.Instance.CharacterManager.GetCharacter(0));

		for(int activeFigureIndex = 0; activeFigureIndex < _sortedFigures.Count || _sortingRequired; activeFigureIndex++)
		{
			// Sort all figures in initiative order
			if(_sortingRequired)
			{
				activeFigureIndex = 0;

				_sortedFigures.Clear();
				_sortedFigures.AddRange(GameController.Instance.Map.Figures);

				_sortedFigures.Sort((turnTakerA, turnTakerB) =>
					turnTakerA.Initiative.SortingInitiative.CompareTo(turnTakerB.Initiative.SortingInitiative));

				_sortingRequired = false;
			}

			Figure figure = _sortedFigures[activeFigureIndex];
			if(!figure.CanTakeTurn)
			{
				continue;
			}

			GameController.Instance.Map.SetTurnTaker(figure);
			await figure.TakeFullTurn();

			GameController.Instance.ResetRelevantTurnTaker();

			await GDTask.DelayFastForwardable(0.5f);

			if(activeFigureIndex + 1 < _sortedFigures.Count)
			{
				ScenarioEvents.NextActiveFigure.Parameters nextActiveFigureParameters =
					await ScenarioEvents.NextActiveFigureEvent.CreatePrompt(
						new ScenarioEvents.NextActiveFigure.Parameters(figure, _sortedFigures[activeFigureIndex+1]));

				if(nextActiveFigureParameters.SortingRequired)
            	{
            	    _sortingRequired = true;
            	}
			}
		}

		GameController.Instance.Map.SetTurnTaker(null);

		// End of round
		// Trigger any end-of-round effects in the scenario rules or on items or character ability cards
		await ScenarioEvents.RoundEndedEvent.CreatePrompt(
			new ScenarioEvents.RoundEnded.Parameters(GameController.Instance.ScenarioPhaseManager.RoundIndex),
			GameController.Instance.CharacterManager.GetCharacter(0));

		// If any drawn attack modifier card or monster ability card has a shuffle icon, shuffle the corresponding discard pile back into the deck
		GameController.Instance.MonsterAMDCardDeck.ReshuffleIfMarked();
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			character.AMDCardDeck.ReshuffleIfMarked();
		}

		foreach(MonsterGroup monsterGroup in GameController.Instance.Map.MonsterGroups)
		{
			monsterGroup.MonsterAbilityCardDeck.ReshuffleIfMarked();
		}

		// If any character ability card in a character’s active area has a round bonus, place it in their discard pile or lost pile, depending on whether the action has a lost icon
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			for(int i = character.Cards.Count - 1; i >= 0; i--)
			{
				AbilityCard card = character.Cards[i];
				if(card.CardState == CardState.Round || card.CardState == CardState.RoundLoss)
				{
					await AbilityCmd.DiscardOrLose(card);
				}
			}

			for(int i = character.Summons.Count - 1; i >= 0; i--)
			{
				Summon summon = character.Summons[i];
				await summon.RemoveActionFromActive();
			}
		}

		foreach(MonsterGroup monsterGroup in GameController.Instance.Map.MonsterGroups)
		{
			await monsterGroup.RemoveCard();
			monsterGroup.AbilityCardInfusedElements.Clear();
			monsterGroup.AbilityCardConsumedElements.Clear();
		}

		// Any character who has at least two cards in their discard pile may perform a short rest (but nah we ain't doing that here)

		GameController.Instance.ElementManager.WaneAll();

		foreach(Figure figure in _sortedFigures)
		{
			figure.RoundEnd();
		}

		GameController.Instance.Map.FigureAddedEvent -= OnFigureAdded;
		GameController.Instance.Map.FigureRemovedEvent -= OnFigureRemoved;
	}

	private void UpdateFigures()
	{
		_sortingRequired = true;
	}

	private void OnFigureAdded(Figure figure)
	{
		UpdateFigures();
	}

	private void OnFigureRemoved(Figure figure)
	{
		UpdateFigures();
	}
}