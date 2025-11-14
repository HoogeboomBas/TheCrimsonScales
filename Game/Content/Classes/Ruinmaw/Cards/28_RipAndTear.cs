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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
				.WithOnAbilityEnded(async state =>
					{
						if (state.Performed && IsSated(state.Performer))
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
							parameters.AbilityState.SingleTargetAddCondition(parameters.AbilityState.UniqueTargetedFigures.Count == 0 ? Conditions.Rupture : Conditions.Wound1);
							await GDTask.CompletedTask;
						})
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
							if(state.UseSlotIndex == 0)
							{
								await SateRuinmaw(applyParameters.Performer);
							}
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
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				])
				//TODO: Fix use slot positioning
				.Build())
		];
		
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}