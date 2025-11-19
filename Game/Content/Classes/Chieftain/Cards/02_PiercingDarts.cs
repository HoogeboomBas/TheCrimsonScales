using System.Collections.Generic;
using Fractural.Tasks;

public class PiercingDarts : ChieftainCardModel<PiercingDarts.CardTop, PiercingDarts.CardBottom>
{
	public override string Name => "Piercing Darts";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 2;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithTargets(2)
				.WithPierce(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAddCondition(Conditions.Poison1);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Poison1))}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state => 
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
								ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

							if(isMountedCheckParameters.IsMounted)
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);
								parameters.AbilityState.SingleTargetAdjustPierce(1);
							}

							await GDTask.CompletedTask;
						}
					);
					
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state => 
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()
			),
		];

		protected override bool Round => true;
	}
}