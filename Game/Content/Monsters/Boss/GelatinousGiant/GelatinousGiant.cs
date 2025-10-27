using System.Collections.Generic;

public class GelatinousGiant : BloodOoze
{
	public override string Name => "Gelatinous Giant";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => GelatinousGiantAbilityCard.Deck;
}