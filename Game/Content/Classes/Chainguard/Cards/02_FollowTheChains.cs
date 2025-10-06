using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;

public class FollowTheChains : ChainguardCardModel<FollowTheChains.CardTop, FollowTheChains.CardBottom>
{
	public override string Name => "Follow the Chains";
	public override int Level => 1;
	public override int Initiative => 19;
	protected override int AtlasIndex => 12 - 2;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPush(1)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPush(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
						parameters =>
							parameters.AbilityState == state &&
							!RangeHelper.GetFiguresInRange(parameters.Hex, 1, false, false)
										.Any(figure => state.Performer.EnemiesWith(figure) && figure.HasCondition(Chainguard.Shackle)),
						parameters =>
						{
							parameters.SetCannotStopAt();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.Build()
			),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithCustomCanApply(parameters => parameters.AbilityState.Performer.HasCondition(Chainguard.Shackle))
				.Build()
			),
		];

		protected override bool Round => true;
	}
}