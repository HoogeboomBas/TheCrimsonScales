using System.Collections.Generic;

public abstract class Imp : MonsterModel
{
	public override IEnumerable<MonsterAbilityCardModel> Deck => ImpAbilityCard.Deck;
}