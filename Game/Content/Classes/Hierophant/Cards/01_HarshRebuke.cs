using System.Collections.Generic;
using Fractural.Tasks;

public class HarshRebuke : HierophantCardModel<HarshRebuke.CardTop, HarshRebuke.CardBottom>
{
	public override string Name => "Harsh Rebuke";
	public override int Level => 1;
	public override int Initiative => 44;
	protected override int AtlasIndex => 29 - 1;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(3, range: 2)),

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
			new AbilityCardAbility(new MoveAbility(3,
				duringMovementSubscriptions:
				[
					ScenarioEvent<ScenarioEvents.DuringMovement.Parameters>.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(1);
							applyParameters.AbilityState.SetCustomValue(this, "EarthConsumed", true);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")
					),
					ScenarioEvent<ScenarioEvents.DuringMovement.Parameters>.Subscription.ConsumeElement(Element.Light,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(1);
							applyParameters.AbilityState.SetCustomValue(this, "LightConsumed", true);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}")
					)
				]
			)),

			new AbilityCardAbility(new ShieldAbility(1,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "EarthConsumed");
				},
				onAbilityEndedPerformed: async state =>
				{
					state.ActionState.SetOverrideRound();

					await GDTask.CompletedTask;
				}
			)),

			new AbilityCardAbility(new HealAbility(1, range: 3,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "LightConsumed");
				}
			))
		];
	}
}