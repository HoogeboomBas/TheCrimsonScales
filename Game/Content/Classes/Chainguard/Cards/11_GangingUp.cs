using System.Collections.Generic;
using System.Linq;

public class GangingUp : ChainguardCardModel<GangingUp.CardTop, GangingUp.CardBottom>
{
	public override string Name => "Ganging Up";
	public override int Level => 1;
	public override int Initiative => 74;
	protected override int AtlasIndex => 12 - 11;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Chainguard.Shackle)
				.Build()
			),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithGetAbilities(state =>
				[
					AttackAbility.Builder().WithDamage(2).Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false);
					figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
				})
				.WithTarget(Target.Enemies)
				.Build()
			),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithCustomGetTargets((state, figures) =>
						{
							IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(grantAbilityState.Performer.Hex, 1, includeOrigin: false);
							figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(grantAbilityState.Performer) 
																			&& figure.HasCondition(Chainguard.Shackle)));
						})
						.WithTarget(Target.Enemies)
						.Build()
				])
				.Build()
			),
		];
	}
}