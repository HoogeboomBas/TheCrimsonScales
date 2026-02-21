using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class HollowEmbrace : HollowpactCardModel<HollowEmbrace.CardTop, HollowEmbrace.CardBottom>
{
	public override string Name => "Hollow Embrace";
	public override int Level => 1;
	public override int Initiative => 53;
	protected override int AtlasIndex => 12;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(2)
						.WithRange(2)
						.WithTargets(2)
						.WithConditions(Conditions.Wound1)
						.Build()
				])
				.WithOnAbilityEndedPerformed(async grantState =>
				{
					int targetedEnemies = grantState.GrantAbilityActionStates.First().AbilityStates.Count(attackState => attackState.Performed);

					if(targetedEnemies > 0)
					{
						await AbilityCmd.SufferDamage(grantState, grantState.Target, targetedEnemies);
						//TODO: Produce
					}
				})
				.Build()),
			/*
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(new DynamicInt<SufferDamageAbility.State>(state =>
				{
					GrantAbility.State grantState = state.ActionState.GetAbilityState<GrantAbility.State>(0);
					
					return grantState.GrantAbilityActionStates.First().AbilityStates.Count(attackState => attackState.Performed);
				}))
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					
					GrantAbility.State grantState = state.ActionState.GetAbilityState<GrantAbility.State>(0);

					if(!grantState.Performed || grantState.GrantAbilityActionStates.Count == 0)
					{
						return false;
					}

					return grantState.GrantAbilityActionStates.First().AbilityStates.Any(attackState => attackState.Performed);
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					//TODO: Produce

					await GDTask.CompletedTask;
				})
				.WithMandatory(true)
				.Build())
				*/
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(3)
				.WithDuringHealSubscription(ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Dark,
					applyFunction: async applyParameters =>
					{
						applyParameters.AbilityState.AbilityAdjustHealValue(1);
						await AbilityCmd.AddCondition(applyParameters.AbilityState, applyParameters.AbilityState.Target, Conditions.Regenerate);

						await AbilityCmd.GainXP(applyParameters.Performer, 1);
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}, " +
					                                                            $"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}")))
				.Build()),
		];
	}
}