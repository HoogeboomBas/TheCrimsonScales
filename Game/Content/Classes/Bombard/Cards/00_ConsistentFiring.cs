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
						new AttackAbility(1, rangeType: RangeType.Range, targetHex: hex),
						new AttackAbility(1, rangeType: RangeType.Range, targetHex: hex, pierce: 1),
						new AttackAbility(1, rangeType: RangeType.Range, targetHex: hex, pierce: 2),
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

			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.5f, 0.85f), GainXP)],
				async state =>
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
				},
				async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}))
		];

		protected override bool Persistent => true;
	}
}