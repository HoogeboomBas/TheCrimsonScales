using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Devotion : HierophantPrayerCardModel<Devotion.CardTop, Devotion.CardBottom>
{
	public override string Name => "Devotion";
	protected override int AtlasIndex => 7 - 1;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AddRetaliate(2, 1);
						}
					);

					//state.Performer.UpdateRetaliate();

					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						canApplyParameters =>
						{
							return
								canApplyParameters.RetaliatingFigure == state.Performer &&
								canApplyParameters.AbilityState.SingleTargetRangeType == RangeType.Melee;
						},
						async applyParameters =>
						{
							applyParameters.AdjustRetaliate(2);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

						//state.Performer.UpdateRetaliate();

						ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.398f, 0.296f)),
						new UseSlot(new Vector2(0.603f, 0.296f))
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(1).Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
		protected override bool Round => true;
	}
}