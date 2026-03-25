using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StrengthElixir : BrightsparkCardModel<StrengthElixir.CardTop, StrengthElixir.CardBottom>
{
	public override string Name => "Strength Elixir";
	public override int Level => 4;
	public override int Initiative => 19;
	protected override int AtlasIndex => 19;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ConditionAbility.Builder()
						.WithConditions(Conditions.Strengthen)
						.WithTarget(Target.Self)
						.Build(),
					ShieldAbility.Builder()
						.WithShieldValue(1)
						.Build()
				])
				.WithRange(2)
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ActionState previousAttackActionState = null;
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Target == state.Performer &&
						              parameters.AbilityState.ActionState != previousAttackActionState,
						async parameters =>
						{
							previousAttackActionState = parameters.AbilityState.ActionState;
							ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.Figure.AlliedWith(state.Performer, true) && canApplyParameters.FromAttack &&
								                      canApplyParameters.PotentialAbilityState.ActionState == parameters.AbilityState.ActionState,
								async applyParameters =>
								{
									applyParameters.AdjustShield(1);

									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.ActionEndedEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.ActionState == parameters.AbilityState.ActionState,
								async applyParameters =>
								{
									ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
									ScenarioEvents.ActionEndedEvent.Unsubscribe(state, this);

									await GDTask.CompletedTask;
								});

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29185185f, 0.81269836f)),
					new UseSlot(new Vector2(0.49777776f, 0.81269836f)),
					new UseSlot(new Vector2(0.70592594f, 0.81269836f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
	}
}