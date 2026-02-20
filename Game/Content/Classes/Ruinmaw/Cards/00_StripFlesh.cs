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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Ruinmaw.Empower)
				.Build())
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
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(4);
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
				.WithUseSlot(new UseSlot(new Vector2(0.47200036f, 0.8174985f), SateRuinmaw))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}