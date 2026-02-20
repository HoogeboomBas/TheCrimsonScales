using System.Collections.Generic;
using System.Linq;
using Godot;

public class GangingUp : ChainguardCardModel<GangingUp.CardTop, GangingUp.CardBottom>
{
	public override string Name => "Ganging Up";
	public override int Level => 1;
	public override int Initiative => 74;
	protected override int AtlasIndex => 12 - 11;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.51381016f, 0.18770726f)))
				.WithConditions(Chainguard.Shackle)
				.Build()
			),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(2, new AttackDiamond(this, new Vector2(0.62174934f, 0.3972468f)))
						.Build()
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(3, new AttackDiamond(this, new Vector2(0.31581503f, 0.80522656f)))
						.WithCustomGetTargets((state, figures) =>
						{
							GrantAbility.State grantAbilityState = state.ActionState.ParentActionState.GetAbilityState<GrantAbility.State>(0);
							IEnumerable<Figure> adjacentFigures =
								RangeHelper.GetFiguresInRange(grantAbilityState.Performer.Hex, 1, includeOrigin: false);
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