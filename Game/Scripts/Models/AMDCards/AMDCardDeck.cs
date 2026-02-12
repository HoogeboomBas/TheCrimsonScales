using System.Collections.Generic;

public class AMDCardDeck : CardDeck<AMDCard>
{
	public AMDCardOwner Owner { get; }

	public AMDCardDeck(IEnumerable<AMDCard> cards, AMDCardOwner owner)
		: base(cards)
	{
		Owner = owner;
	}

	public static List<AMDCard> GetDefaultDeckCards(AMDCardOwner owner)
	{
		return new List<AMDCard>()
		{
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusZeroAMDCard>(), owner),

			new AMDCard(ModelDB.AMDCard<PlusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusOneAMDCard>(), owner),

			new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), owner),

			new AMDCard(ModelDB.AMDCard<MinusTwoAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<PlusTwoAMDCard>(), owner),

			new AMDCard(ModelDB.AMDCard<NullAMDCard>(), owner),
			new AMDCard(ModelDB.AMDCard<CritAMDCard>(), owner),
		};
	}

	public void AddMinusOne()
	{
		AddCard(new AMDCard(ModelDB.AMDCard<MinusOneAMDCard>(), Owner), true);
	}
}