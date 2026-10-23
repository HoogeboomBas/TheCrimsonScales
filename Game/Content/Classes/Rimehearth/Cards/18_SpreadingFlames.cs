using System.Collections.Generic;
using Fractural.Tasks;

public class SpreadingFlames : RimehearthCardModel<SpreadingFlames.CardTop, SpreadingFlames.CardBottom>
{
	public override string Name => "Spreading Flames";
	public override int Level => 4;
	public override int Initiative => 60;
	protected override int AtlasIndex => 18;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.WithOnAbilityEndedPerformed(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => !state.Performer.HasWound(),
						async _ =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.HasWound() && parameters.Performer.HasCondition(Conditions.Chill),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(parameters.Performer.GetCondition(Conditions.Chill)
								.StackCount);
							await GDTask.CompletedTask;
						}))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}
}