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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithConditions([Ruinmaw.Empower, Ruinmaw.Empower])
				.WithConditionalAbilityCheck(async state =>
				{
					Figure adjacentAlly = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1).Where(figure => figure.AlliedWith(state.Performer)));
					}, hintText: () => $"Select an ally to suffer {Icons.HintText(Icons.Damage)}2");
					if(adjacentAlly != null)
					{
						await AbilityCmd.SufferDamage(state, adjacentAlly, 2);
						return true;
					}

					return false;
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
				.WithUseSlot(new UseSlot(new Vector2(0.48150024f, 0.41998473f)))
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Target == state.Performer &&
							state.Performer is Ruinmaw ruinmaw &&
							ruinmaw.Sated &&
							canApplyParameters.ConditionModel.IsNegative,
						async parameters =>
						{
							parameters.SetPrevented(true);

							await GDTask.CompletedTask;
						}
					);

					if(state.Performer is Ruinmaw ruinmaw)
					{
						ruinmaw.SateEvent += OnSated;
					}

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);
					if(state.Performer is Ruinmaw ruinmaw)
					{
						ruinmaw.SateEvent -= OnSated;
					}

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Persistent => true;

		private async GDTask OnSated(Ruinmaw ruinmaw)
		{
			await AbilityCmd.RemoveAllNegativeConditions(ruinmaw);
		}
	}
}