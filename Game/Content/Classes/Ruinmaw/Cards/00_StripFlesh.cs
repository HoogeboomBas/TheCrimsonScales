using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StripFlesh : RuinmawCardModel<StripFlesh.CardTop, StripFlesh.CardBottom>
{
	public override string Name => "Strip Flesh";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 0;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.EmpowerRuinmaw)
				.Build())
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
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(4);
							applyParameters.AbilityState.SingleTargetSetHasAdvantage();
							await SateRuinmaw(applyParameters.Performer);
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

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}