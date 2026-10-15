using System.Collections.Generic;
using System.Linq;
using Godot;

public class LivingBonesScenario55 : LivingBones
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Where(trait => trait is not TargetsTrait)
				.ToArray()
			})
			.ToArray();

	public override IEnumerable<MonsterAbilityCardModel> Deck => [ModelDB.MonsterAbilityCard<LivingBonesAllyAbilityCard>()];

	private class LivingBonesAllyAbilityCard : MonsterAbilityCardModel
	{
		public override int Initiative => 50;
		public override int CardIndex => 0;
		public override string CardsAtlasPath => "res://Content/Monsters/LivingBones/Scenario55Card.jpg";
		public override int ColumnCount => 1;
		public override int RowCount => 1;

		public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
		[
			new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
			new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		];
	}
}