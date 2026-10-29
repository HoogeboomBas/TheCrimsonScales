using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MakusDeadlyAim : IncarnateCardModel<MakusDeadlyAim.CardTop, MakusDeadlyAim.CardBottom>
{
	public override string Name => "Maku's Deadly Aim";
	public override int Level => 1;
	public override int Initiative => 40;
	protected override int AtlasIndex => 6;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.23157895f)))
				.WithDuringAttackSubscriptions(
				[
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}),
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(2);

							await GDTask.CompletedTask;
						})
				])
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6209548f, 0.6565097f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithTarget(Target.Allies)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await state.ActionState.RequestDiscardOrLose();
						});

					state.ActionState.SetOverrideRound();

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.Build())
		];
	}
}