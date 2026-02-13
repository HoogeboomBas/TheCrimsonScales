using System.Collections.Generic;

public class HarrowerIcecrawlers : HarrowerInfester
{
	public override string Name => "Harrower Icecrawlers";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<HarrowerIcecrawlers>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => HarrowerIcecrawlersAbilityCard.Deck;
}