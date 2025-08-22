using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ThrowingDaggers : MirefootCardModel<ThrowingDaggers.CardTop, ThrowingDaggers.CardBottom>
{
	public override string Name => "Throwing Daggers";
	public override int Level => 2;
	public override int Initiative => 49;
	protected override int AtlasIndex => 13;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new UseSlotAbility(
				[
					new UseSlot(new Vector2(0.2869934f, 0.28600082f)), new UseSlot(new Vector2(0.5f, 0.28600082f)),
					new UseSlot(new Vector2(0.7025001f, 0.28600082f), GainXP)
				],
				async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							parameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetRangeType(RangeType.Range);
							parameters.AbilityState.SingleTargetAdjustRange(2);

							await state.AdvanceUseSlot();
						},
						EffectType.SelectableMandatory,
						canApplyMultipleTimesDuringSubscription: true,
						effectButtonParameters: new TextEffectButton.Parameters($"+2{Icons.Inline(Icons.Range)}"),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			)),

			new AbilityCardAbility(new AttackAbility(2))
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).WithMoveType(MoveType.Jump).Build())
		];
	}
}