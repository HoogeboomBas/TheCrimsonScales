using System.Collections.Generic;

public class TerrorDrone : StoneGolem
{
	public override string Name => "Terror Drone";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<StoneGolem>();
	public override IEnumerable<MonsterAbilityCardModel> Deck => DeepTerrorAbilityCard.Deck;
}