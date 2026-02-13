using System.Collections.Generic;
using System.Linq;

public class BlackBear : CaveBear
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new AttackersGainDisadvantageTrait())
				.ToArray()
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new AttackersGainDisadvantageTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Black Bear";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<CaveBear>();
	public override IEnumerable<MonsterAbilityCardModel> Deck => NightDemonAbilityCard.Deck;
}