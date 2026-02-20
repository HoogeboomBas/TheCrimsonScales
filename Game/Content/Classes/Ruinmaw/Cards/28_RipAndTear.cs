using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RipAndTear : RuinmawCardModel<RipAndTear.CardTop, RipAndTear.CardBottom>
{
	public override string Name => "Rip and Tear";
	public override int Level => 9;
	public override int Initiative => 20;
	protected override int AtlasIndex => 28;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithTargets(2)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(1);
							((AttackAbility.State)parameters.AbilityState).AbilitySetHasAdvantage();
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustPierce(2);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
					{
						if(IsSated(state.Performer))
						{
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.AbilityState.UniqueTargetedFigures.Count < 2,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(parameters.AbilityState.UniqueTargetedFigures.Count == 0
								? Conditions.Rupture
								: Conditions.Wound1);
							await GDTask.CompletedTask;
						})
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer &&
						                      (canApplyParameters.AbilityState.Target.HasWound() ||
						                       canApplyParameters.AbilityState.Target.HasCondition(Conditions.Rupture)),
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(4);
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
					new UseSlot(new Vector2(0.2495004f, 0.755998f), SateRuinmaw),
					new UseSlot(new Vector2(0.45350015f, 0.755998f)),
					new UseSlot(new Vector2(0.66049993f, 0.755998f), GainXP),
					new UseSlot(new Vector2(0.19800027f, 0.8769984f)),
					new UseSlot(new Vector2(0.4045f, 0.8769984f)),
					new UseSlot(new Vector2(0.6075001f, 0.8769984f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}