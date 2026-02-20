using System.Collections.Generic;
using System.Linq;
using Godot;

public class AgonizingClamp : ChainguardLevelUpCardModel<AgonizingClamp.CardTop, AgonizingClamp.CardBottom>
{
	public override string Name => "Agonizing Clamp";
	public override int Level => 2;
	public override int Initiative => 57;
	protected override int AtlasIndex => 15 - 1;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.3550224f, 0.34218287f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.61960894f, 0.7218475f)))
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(3)
				.WithRange(1)
				.Build())
		];
	}
}