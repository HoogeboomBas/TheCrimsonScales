using System.Collections.Generic;
using System.Linq;

public class AgonizingClamp : ChainguardLevelUpCardModel<AgonizingClamp.CardTop, AgonizingClamp.CardBottom>
{
	public override string Name => "Agonizing Clamp";
	public override int Level => 2;
	public override int Initiative => 57;
	protected override int AtlasIndex => 15 - 1;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false);
					figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
				})
				.Build()
			),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(3)
				.WithRange(1)
				.Build())
		];
	}
}