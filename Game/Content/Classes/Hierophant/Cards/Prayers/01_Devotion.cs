using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Devotion : HierophantPrayerCardModel<Devotion.CardTop, Devotion.CardBottom>
{
	public override string Name => "Devotion";
	protected override int AtlasIndex => 7 - 1;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					await AbilityCmd.AddRetaliate(state.Performer, this, 2, 3,
						customCanApplyParameters => customCanApplyParameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
						customApply: async customApplyParameters =>
						{
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						AbilityCmd.RemoveRetaliate(state.Performer, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.3835002f, 0.2875001f)),
						new UseSlot(new Vector2(0.5855027f, 0.2875001f))
					]
				)
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(1).Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override bool Round => true;
	}
}