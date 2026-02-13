using System.Collections.Generic;

public class HailDemon : WindDemon
{
	public override string Name => "Hail Demon";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<WindDemon>();
	public override IEnumerable<MonsterAbilityCardModel> Deck => HailDemonAbilityCard.Deck;
}