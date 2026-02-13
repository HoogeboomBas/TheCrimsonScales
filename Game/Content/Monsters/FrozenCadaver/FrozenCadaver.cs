using System.Collections.Generic;

public class FrozenCadaver : LivingCorpse
{
	public override string Name => "Frozen Cadaver";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<LivingCorpse>();

	public override IEnumerable<MonsterAbilityCardModel> Deck => FrozenCadaverAbilityCard.Deck;
}