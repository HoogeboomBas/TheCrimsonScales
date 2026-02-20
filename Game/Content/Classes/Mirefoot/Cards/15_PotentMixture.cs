using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PotentMixture : MirefootCardModel<PotentMixture.CardTop, PotentMixture.CardBottom>
{
	public override string Name => "Potent Mixture";
	public override int Level => 3;
	public override int Initiative => 17;
	protected override int AtlasIndex => 15;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();

							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison3);

							await AbilityCmd.GainXP(state.Performer, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6215359f, 0.67549807f)))
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state,
						this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							int poisonAmount = 0;
							if(parameters.AbilityState.Target.HasCondition(Conditions.Poison4))
							{
								poisonAmount = 4;
							}
							else if(parameters.AbilityState.Target.HasCondition(Conditions.Poison3))
							{
								poisonAmount = 3;
							}
							else if(parameters.AbilityState.Target.HasCondition(Conditions.Poison2))
							{
								poisonAmount = 2;
							}
							else if(parameters.AbilityState.Target.HasCondition(Conditions.Poison1))
							{
								poisonAmount = 1;
							}

							parameters.AbilityState.SingleTargetAdjustAttackValue(poisonAmount);

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
				.WithUseSlot(new UseSlot(null))
				.Build())
		];

		public override bool Round => true;
	}
}