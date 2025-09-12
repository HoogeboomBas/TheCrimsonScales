using System.Collections.Generic;
using Fractural.Tasks;

public class LoyalCompanion : FireKnightCardModel<LoyalCompanion.CardTop, LoyalCompanion.CardBottom>
{
	public override string Name => "Loyal Companion";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 12 - 11;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 4,
					Move = 2,
					Attack = 1,
					Traits = [new AdjacentAlliesGainAdvantageTrait()]
				})
				.WithName("Spotted Hound")
				.WithTexturePath("res://Content/Classes/FireKnight/SpottedHound.jpg")
				.Build()
			),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					SummonAbility.State summonAbilityState = state.ActionState.GetAbilityState<SummonAbility.State>(0);
					ActionState actionState = new ActionState(summonAbilityState.Summon,
					[
						ConditionAbility.Builder()
							.WithConditions(Conditions.Bless)
							.WithTarget(Target.Allies | Target.TargetAll)
							.WithRange(2)
							.Build()
					], state.ActionState);
					await actionState.Perform();

					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							moveAbilityState.AddJump();

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}
}