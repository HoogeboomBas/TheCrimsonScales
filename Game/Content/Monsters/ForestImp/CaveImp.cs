using System.Collections.Generic;
using System.Linq;

public class CaveImp : ForestImp
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * 2
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * 2
			})
			.ToArray();

	public override string Name => "Cave Imp";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<ForestImp>();
	public override IEnumerable<MonsterAbilityCardModel> Deck => AncientArtilleryAbilityCard.Deck;
}