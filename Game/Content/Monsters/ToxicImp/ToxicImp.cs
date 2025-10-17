using System.Collections.Generic;

public class ToxicImp : ForestImp
{
	public override string Name => "Toxic Imp";

	public override IEnumerable<MonsterAbilityCardModel> Deck => ToxicImpAbilityCard.Deck;
}