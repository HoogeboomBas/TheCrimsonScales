using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DoubleKO : ChainguardLevelUpCardModel<DoubleKO.CardTop, DoubleKO.CardBottom>
{
	public override string Name => "Double K.O.";
	public override int Level => 4;
	public override int Initiative => 92;
	protected override int AtlasIndex => 15 - 5;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithConditionalAbilityCheck(async state => 
				{
					bool killedAnEnemy = state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count > 0;

					if(killedAnEnemy)
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
					return killedAnEnemy;
				})
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(applyParameters.AbilityState.SingleTargetAttackValue);
							await AbilityCmd.GainXP(state.Performer, 2);
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.85f)))
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}