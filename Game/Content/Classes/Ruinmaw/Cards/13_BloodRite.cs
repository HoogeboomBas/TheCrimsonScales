using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BloodRite : RuinmawCardModel<BloodRite.CardTop, BloodRite.CardBottom>
{
	public override string Name => "Blood Rite";
	public override int Level => 1;
	public override int Initiative => 67;
	protected override int AtlasIndex => 13;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure adjacentAlly = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure => figure.AlliedWith(state.Performer)));
					}, hintText: $"Select an ally to suffer {Icons.HintText(Icons.Damage)}2");
					if (adjacentAlly != null)
					{
						await AbilityCmd.SufferDamage(state, adjacentAlly, 2);
						state.SetCustomValue(this, "Ally Suffered Damage", true);
					}
					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.EmpowerRuinmaw, Conditions.EmpowerRuinmaw)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<bool>(this, "Ally Suffered Damage");
				})
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustPierce(1);
							applyParameters.AbilityState.SingleTargetSetHasAdvantage();
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
				//TODO: Fix use slot positioning
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Target == state.Performer &&
							state.Performer is Ruinmaw ruinmaw && ruinmaw.Sated && canApplyParameters.Condition.IsNegative,
						async parameters =>
						{
							parameters.SetPrevented(true);

							await GDTask.CompletedTask;
						});

					if (state.Performer is Ruinmaw ruinmaw)
                    {
						ruinmaw.SateEvent += RemoveConditions;
                    }

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);
					if (state.Performer is Ruinmaw ruinmaw)
					{
						ruinmaw.SateEvent -= RemoveConditions;
					}
					
					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override bool Persistent => true;

		private async void RemoveConditions(Ruinmaw ruinmaw)
        {
			foreach(ConditionModel condition in ruinmaw.Conditions)
			{
				await AbilityCmd.RemoveCondition(ruinmaw, condition);
			}
        }
	}
}