using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Salvation : HierophantPrayerCardModel<Salvation.CardTop, Salvation.CardBottom>
{
	public override string Name => "Salvation";
	protected override int AtlasIndex => 7 - 7;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AdjustShield(2);
						}
					);

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.FromAttack,
						async parameters =>
						{
							parameters.AdjustShield(2);

							await state.AdvanceUseSlot();
						}
					);

					AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
						ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

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
			new AbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
		protected override bool Round => true;
	}
}