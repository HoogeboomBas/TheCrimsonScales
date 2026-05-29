using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

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
					if(grantState.GrantAbilityActionStates.First().AbilityStates.First() is AttackAbility.State attackState && attackState.Performed)
					{
						int targetedEnemies = attackState.UniqueTargetedFigures.Count;

						if(targetedEnemies > 0)
						{
							await AbilityCmd.SufferDamage(grantState, grantState.Target, targetedEnemies);
							await GainVoidEnergy(grantState);
						}
					}
				})
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.60472214f, 0.71606654f)))
				.WithDuringHealSubscription(ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Dark,
					applyFunction: async applyParameters =>
					{
						applyParameters.AbilityState.AbilityAdjustHealValue(1);
						applyParameters.AbilityState.AbilityAddCondition(Conditions.Regenerate);

						await AbilityCmd.GainXP(applyParameters.Performer, 1);
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}, " +
					                                                            $"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}")))
				.Build()),
		];
	}
}