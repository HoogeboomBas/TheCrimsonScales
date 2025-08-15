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
			new AbilityCardAbility(new SummonAbility(new SummonStats()
				{
					Health = 4,
					Move = 2,
					Attack = 1,
					Traits = [new AdjacentAlliesGainAdvantageTrait()]
				},
				"Spotted Hound", "res://Content/Classes/FireKnight/SpottedHound.jpg"
			)),

			new AbilityCardAbility(new OtherAbility(async state =>
				{
					SummonAbility.State summonAbilityState = state.ActionState.GetAbilityState<SummonAbility.State>(0);
					ActionState actionState = new ActionState(summonAbilityState.Summon,
					[
						new ConditionAbility([Conditions.Bless], target: Target.Allies | Target.TargetAll, range: 2)
					], state.ActionState);
					await actionState.Perform();
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(4,
				abilityStartedSubscriptions:
				[
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							moveAbilityState.AddJump();

							await GDTask.CompletedTask;
						}
					)
				]
			))
		];
	}
}