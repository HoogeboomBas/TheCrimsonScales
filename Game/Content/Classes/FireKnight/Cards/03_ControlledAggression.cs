using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ControlledAggression : FireKnightCardModel<ControlledAggression.CardTop, ControlledAggression.CardBottom>
{
	public override string Name => "Controlled Aggression";
	public override int Level => 1;
	public override int Initiative => 75;
	protected override int AtlasIndex => 12 - 3;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer ||
							(state.Performer.AlliedWith(parameters.Performer) &&
							 RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) == 1),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.3759946f, 0.35149997f)),
						new UseSlot(new Vector2(0.5909972f, 0.35149997f), GainXP)
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build())
		];
	}
}