using System.Collections.Generic;

public class ToxicImp : ForestImp
{
	public override string Name => "Toxic Imp";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<ForestImp>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => ToxicImpAbilityCard.Deck;
}