using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FatalFrenzy : RuinmawCardModel<FatalFrenzy.CardTop, FatalFrenzy.CardBottom>
{
	public override string Name => "Fatal Frenzy";
	public override int Level => 2;
	public override int Initiative => 40;
	protected override int AtlasIndex => 15;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if(IsSated(state.Performer))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}

					return IsSated(state.Performer);
				})
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
						                      canApplyParameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Rupture);
							applyParameters.AbilityState.SingleTargetAdjustPush(2);
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
					new UseSlot(new Vector2(0.24700023f, 0.75549895f), SateRuinmaw),
					new UseSlot(new Vector2(0.45450008f, 0.75549895f)),
					new UseSlot(new Vector2(0.6615002f, 0.75499797f), GainXP),
					new UseSlot(new Vector2(0.1965004f, 0.87799823f)),
					new UseSlot(new Vector2(0.4040005f, 0.87799823f)),
					new UseSlot(new Vector2(0.6079994f, 0.87799823f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}