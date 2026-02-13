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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6215293f, 0.24632105f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							IEnumerable<Figure> figures = RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, includeOrigin: false);

							parameters.AbilityState.SingleTargetAdjustAttackValue(2 *
							                                                      figures.Count(figure => figure.EnemiesWith(parameters.Performer)));

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(3, new AttackDiamond(this, new Vector2(0.62198937f, 0.8269419f)))
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
}