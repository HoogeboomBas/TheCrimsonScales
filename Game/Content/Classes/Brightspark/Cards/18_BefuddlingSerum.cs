using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BefuddlingSerum : BrightsparkCardModel<BefuddlingSerum.CardTop, BefuddlingSerum.CardBottom>
{
	public override string Name => "Befuddling Serum";
	public override int Level => 4;
	public override int Initiative => 49;
	protected override int AtlasIndex => 18;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.34814814f, 0.28994706f)))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithConditions(Conditions.Muddle)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Muddle);
							await GDTask.CompletedTask;
						});
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							switch(state.UseSlotIndex)
							{
								case 0:
									parameters.AbilityState.SingleTargetAdjustPush(3);
									break;
								case 1:
									parameters.AbilityState.SingleTargetAdjustPierce(3);
									break;
								case 2:
									parameters.AbilityState.SingleTargetAdjustAttackValue(3);
									break;
								case 3:
									parameters.AbilityState.AdjustTargets(1);
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}

							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16296296f, 0.80529094f)),
					new UseSlot(new Vector2(0.3711111f, 0.80529094f), GainXP),
					new UseSlot(new Vector2(0.5785185f, 0.80529094f)),
					new UseSlot(new Vector2(0.78962964f, 0.80529094f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}