using System.Collections.Generic;

public class BloodOoze : Ooze
{
	public override string Name => "Blood Ooze";

	public override IEnumerable<MonsterAbilityCardModel> Deck => BloodOozeAbilityCard.Deck;
}