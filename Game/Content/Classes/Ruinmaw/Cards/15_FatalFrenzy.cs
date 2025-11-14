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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if (IsSated(state.Performer))
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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer && canApplyParameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Rupture);
							applyParameters.AbilityState.SingleTargetAdjustPush(2);
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