using Fractural.Tasks;

public class AMDManager
{
	public int RemainingBlessCount { get; private set; } = 10;
	public int RemainingCharacterCurseCount { get; private set; } = 10;
	public int RemainingMonsterCurseCount { get; private set; } = 10;

	public bool Bless(Figure figure)
	{
		if(RemainingBlessCount == 0)
		{
			return false;
		}

		RemainingBlessCount--;
		AMDCard card = new AMDCard(ModelDB.AMDCard<BlessAMDCard>(), figure.AMDCardDeck.Owner);
		card.DrawnEvent += OnBlessDrawn;
		figure.AMDCardDeck.AddCard(card, true);
		return true;
	}

	public bool Curse(Figure figure)
	{
		AMDCardDeck deck = figure.AMDCardDeck;

		if(deck.Owner != AMDCardOwner.Monsters)
		{
			if(RemainingCharacterCurseCount == 0)
			{
				return false;
			}

			RemainingCharacterCurseCount--;

			AMDCard card = new AMDCard(ModelDB.AMDCard<CurseAMDCard>(), deck.Owner);
			card.DrawnEvent += OnCharacterCurseDrawn;
			figure.AMDCardDeck.AddCard(card, true);
		}
		else
		{
			if(!CurseMonsters())
			{
				return false;
			}
		}

		return true;
	}

	public bool CurseMonsters()
	{
		if(RemainingMonsterCurseCount == 0)
		{
			return false;
		}

		RemainingMonsterCurseCount--;

		AMDCard card = new AMDCard(ModelDB.AMDCard<CurseAMDCard>(), AMDCardOwner.Monsters);
		card.DrawnEvent += OnMonsterCurseDrawn;
		GameController.Instance.MonsterAMDCardDeck.AddCard(card, true);

		return true;
	}

	public async GDTask<bool> Empower(IHasEmpower originalOwner, Figure figure)
	{
		if(originalOwner.RemainingEmpowerCount == 0)
		{
			return false;
		}

		originalOwner.RemainingEmpowerCount--;
		AMDCard card = new AMDCard(originalOwner.CreateEmpower(), figure.AMDCardDeck.Owner, potentialOriginalCardOwner: (Character)originalOwner);
		ScenarioEvents.EmpowerAdded.Parameters empowerAddedParameters =
			await ScenarioEvents.EmpowerAddedEvent.CreatePrompt(
				new ScenarioEvents.EmpowerAdded.Parameters(figure));

		card.DrawnEvent += OnEmpowerDrawn;

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

	private void OnEmpowerDrawn(AMDCard card)
	{
		((IHasEmpower)card.PotentialOriginalCardOwner).RemainingEmpowerCount++;
	}
}