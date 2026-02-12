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
			if(!CurseMonsters())
			{
				return false;
			}
		}
		else
		{
			if(RemainingMonsterCurseCount == 0)
			{
				return false;
			}

			RemainingMonsterCurseCount--;

			AMDCard card = new AMDCard(ModelDB.AMDCard<CurseAMDCard>(), deck.Owner);
			card.DrawnEvent += OnMonsterCurseDrawn;
			figure.AMDCardDeck.AddCard(card, true);
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