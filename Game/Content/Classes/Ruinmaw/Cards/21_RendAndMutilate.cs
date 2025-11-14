using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;

public class RendAndMutilate : RuinmawCardModel<RendAndMutilate.CardTop, RendAndMutilate.CardBottom>
{
	public override string Name => "Rend and Mutilate";
	public override int Level => 5;
	public override int Initiative => 74;
	protected override int AtlasIndex => 21;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(8)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(-4);
							((AttackAbility.State)parameters.AbilityState).AbilityAddConditionPreAbility(Conditions.Wound1);
							_loss = false;
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];

		protected override bool Sate => true;
		protected override int XP => 1;
		private bool _loss = true;
		protected override bool Loss => _loss;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					if(IsSated(state.Performer))
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.AbilityState.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.AbilitySetHasAdvantage();
							parameters.AbilityState.AbilityAdjustAttackValue(3);
							await GDTask.CompletedTask;
						});
					}
					else
					{
                        ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.AbilityState.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.AbilitySetHasAdvantage();
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							await GDTask.CompletedTask;
						});
					}

					await GDTask.CompletedTask;
					
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override bool Round => true;
	}
}