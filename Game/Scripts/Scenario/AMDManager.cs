using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class AMDManager
{
	public List<CharacterStartHex> CharacterStartHexes { get; private set; }

	public List<Character> Characters { get; } = new List<Character>();

	public int RemainingBlessCount = 10;
	public int RemainingCharacterCurseCount = 10;
	public int RemainingMonsterCurseCount = 10;

	public bool Bless(Figure figure)
	{
		if(RemainingBlessCount == 0)
		{
			return false;
		}

		RemainingBlessCount--;
		AMDCard card = new BlessAMDCard("res://Art/AMDs/Other.jpg", 3, 4, 2);
		card.DrawnEvent += OnBlessDrawn;
		figure.AMDCardDeck.AddCard(card, true);
		return true;
	}

	public bool Curse(Figure figure)
	{
		AMDCardDeck deck = figure.AMDCardDeck;

		if(deck.CharacterDeck)
		{
			if(RemainingCharacterCurseCount == 0)
			{
				return false;
			}

			RemainingCharacterCurseCount--;

			AMDCard card = new CurseAMDCard("res://Art/AMDs/Other.jpg", 2, 4, 2);
			card.DrawnEvent += OnCharacterCurseDrawn;
			figure.AMDCardDeck.AddCard(card, true);
		}
		else
		{
			if(RemainingMonsterCurseCount == 0)
			{
				return false;
			}

			RemainingMonsterCurseCount--;

			AMDCard card = new CurseAMDCard("res://Art/AMDs/Other.jpg", 1, 4, 2);
			card.DrawnEvent += OnMonsterCurseDrawn;
			figure.AMDCardDeck.AddCard(card, true);
		}

		return true;
	}

	public async GDTask<bool> Empower(IHasEmpower originalOwner, Figure figure)
	{
		if(originalOwner.RemainingEmpowerCount == 0)
		{
			return false;
		}
		originalOwner.RemainingEmpowerCount--;
		AMDCard card = (AMDCard)Activator.CreateInstance(originalOwner.EmpowerType, originalOwner);
		ScenarioEvents.EmpowerAdded.Parameters empowerAddedParameters =
			await ScenarioEvents.EmpowerAddedEvent.CreatePrompt(
				new ScenarioEvents.EmpowerAdded.Parameters(figure));

		figure.AMDCardDeck.AddCard(card, empowerAddedParameters.ShuffleDrawPile);
		return true;
	}

	private void OnBlessDrawn(AMDCard card)
	{
		RemainingBlessCount++;
	}

	private void OnCharacterCurseDrawn(AMDCard card)
	{
		RemainingCharacterCurseCount++;
	}

	private void OnMonsterCurseDrawn(AMDCard card)
	{
		RemainingMonsterCurseCount++;
	}
}