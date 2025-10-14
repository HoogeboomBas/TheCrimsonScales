using System.Collections.Generic;
using Fractural.Tasks;

public class HarshRebuke : HierophantCardModel<HarshRebuke.CardTop, HarshRebuke.CardBottom>
{
	public override string Name => "Harsh Rebuke";
	public override int Level => 1;
	public override int Initiative => 44;
	protected override int AtlasIndex => 13 - 4;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(3).WithRange(2).Build()),

			new AbilityCardAbility(GivePrayerCardAbility(
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					return attackAbilityState.Performed && attackAbilityState.KilledTargets.Count > 0;
				}, range: 2
			))
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithDuringMovementSubscriptions(
					[
						ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Earth,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AdjustMoveValue(1);
								applyParameters.AbilityState.SetCustomValue(this, "EarthConsumed", true);
								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")
						),
						ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Light,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AdjustMoveValue(1);
								applyParameters.AbilityState.SetCustomValue(this, "LightConsumed", true);
								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")
						)
					]
				)
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "EarthConsumed");
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						state.ActionState.SetOverrideRound();

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithRange(3)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "LightConsumed");
					}
				)
				.Build())
		];
	}
}