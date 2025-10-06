using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ImpendingPower : ChainguardLevelUpCardModel<ImpendingPower.CardTop, ImpendingPower.CardBottom>
{
	public override string Name => "Impending Power";
	public override int Level => 5;
	public override int Initiative => 12;
	protected override int AtlasIndex => 15 - 6;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApply: parameters => state.Performer == parameters.AbilityState.Target,
						apply: async parameters =>
						{
							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription shieldChosenSubscription =
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async applyParameters =>
									{
										ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
											canApplyParameters => canApplyParameters.Figure == state.Performer && canApplyParameters.FromAttack,
											async applyParameters =>
											{
												applyParameters.AdjustShield(2);
												ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

												await GDTask.CompletedTask;
											}
										);
										
										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Shield),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"Gain {Icons.Inline(Icons.Shield)}2 for the attack"),
									effectType: EffectType.SelectableMandatory
								);

							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription retaliateChosenSubscription =
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async applyParameters =>
									{
										ScenarioEvents.RetaliateEvent.Subscribe(state, this,
											canApplyParameters =>
											{
												return canApplyParameters.RetaliatingFigure == state.Performer &&
													RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1;
											},
											async applyParameters =>
											{
												applyParameters.AdjustRetaliate(1);
												ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

												await GDTask.CompletedTask;
											}
										);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Retaliate),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"Gain {Icons.Inline(Icons.Retaliate)}1 for the attack"),
									effectType: EffectType.SelectableMandatory
								);

							await AbilityCmd.GenericChoice(state.Performer, 
								[shieldChosenSubscription, retaliateChosenSubscription], 
								hintText: "Select an effect to gain for the attack:");

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

					ActionState actionState = new ActionState(state.Performer,
					[
						HealAbility.Builder()
							.WithHealValue(state.Slots.Count - state.UseSlotIndex)
							.WithTarget(Target.Self)
							.Build()
					]);

					await actionState.Perform();
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2869934f, 0.30899984f)),
					new UseSlot(new Vector2(0.49549526f, 0.30899984f)),
					new UseSlot(new Vector2(0.70750487f, 0.30899984f)),
					new UseSlot(new Vector2(0.603f, 0.43299824f)),
					new UseSlot(new Vector2(0.39799652f, 0.43299824f))
				])
				.Build())
		];

		protected override int XP => 1;
		protected override bool Round => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state => 
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						canApply: parameters => parameters.Performer == state.Performer && 
								parameters.AbilityState is CreateTrapAbility.State && 
								((CreateTrapAbility.State)parameters.AbilityState).AbilityRange == 1,
						async parameters =>
							{
								((CreateTrapAbility.State)parameters.AbilityState).AbilityAdjustRange(1);
								ScenarioEvents.AbilityStartedEvent.Unsubscribe(state.Performer, this);

								await GDTask.CompletedTask;
							}
						);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state => 
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state.Performer, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state => 
				{
					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => state.Performer == canApplyParameters.Authority,
						async applyParameters =>
						{
							await AbilityCmd.SufferDamage(null, applyParameters.Figure, 2);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state => 
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;

	}
}