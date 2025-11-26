using System.Collections.Generic;

public class HailDemon : WindDemon
{
	public override string Name => "Hail Demon";
	public override IEnumerable<MonsterAbilityCardModel> Deck => HailDemonAbilityCard.Deck;
}