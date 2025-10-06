using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MercilessBeatdown : ChainguardCardModel<MercilessBeatdown.CardTop, MercilessBeatdown.CardBottom>
{
	public override string Name => "Merciless Beatdown";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 12 - 5;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							IEnumerable<Figure> figures = RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, includeOrigin: false);
							
							parameters.AbilityState.AbilityAdjustAttackValue(2 * figures.Count(figure => figure.EnemiesWith(parameters.Performer)));

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ControlAbility.Builder()
				.WithGetAbilities(state =>
				[
					AttackAbility.Builder().WithDamage(3).Build()
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
}