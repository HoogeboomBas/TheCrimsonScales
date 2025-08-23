using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ConsistentFiring : BombardCardModel<ConsistentFiring.CardTop, ConsistentFiring.CardBottom>
{
	public override string Name => "Consistent Firing";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 0;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
					[
						AttackAbility.Builder()
							.WithDamage(1)
							.WithRangeType(RangeType.Range)
							.WithTargetHex(hex)
							.Build(),
						AttackAbility.Builder()
							.WithDamage(1)
							.WithRangeType(RangeType.Range)
							.WithTargetHex(hex)
							.WithPierce(1)
							.Build(),
						AttackAbility.Builder()
							.WithDamage(1)
							.WithRangeType(RangeType.Range)
							.WithTargetHex(hex)
							.WithPierce(2)
							.Build(),
					]
				)
				.WithAbilityCardSide(this)
				.WithRange(3)
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetSetHasAdvantage();
							//await AbilityCmd.DiscardOrLose(AbilityCard);
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.85f), GainXP))
				.Build())
		];

		protected override bool Persistent => true;
	}
}