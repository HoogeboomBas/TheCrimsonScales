using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StationaryEnhancements : BombardCardModel<StationaryEnhancements.CardTop, StationaryEnhancements.CardBottom>
{
	public override string Name => "Stationary Enhancements";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 14;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AdjustShield(2);
						}
					);

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer && canApplyParameters.FromAttack,
						async applyParameters =>
						{
							applyParameters.AdjustShield(2);

							await state.AdvanceUseSlot();
						}
					);

					AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);

						ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.2869934f, 0.3959994f)),
						new UseSlot(new Vector2(0.5f, 0.3959994f)),
						new UseSlot(new Vector2(0.7025001f, 0.3959994f), GainXP)
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							parameters.AbilityState.SingleTargetRangeType == RangeType.Range,
						async parameters =>
						{
							switch(state.UseSlotIndex)
							{
								case 0:
									parameters.AbilityState.SingleTargetAdjustPush(2);
									break;
								case 1:
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
									break;
								case 2:
									parameters.AbilityState.SingleTargetAdjustPierce(3);
									break;
							}

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.2869934f, 0.8740155f)),
						new UseSlot(new Vector2(0.5f, 0.8740155f)),
						new UseSlot(new Vector2(0.7025001f, 0.8740155f), GainXP)
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}
}