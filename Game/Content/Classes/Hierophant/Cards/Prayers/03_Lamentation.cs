using System.Collections.Generic;
using Fractural.Tasks;

public class Lamentation : HierophantPrayerCardModel<Lamentation.CardTopBottom, Lamentation.CardTopBottom>
{
	public override string Name => "Lamentation";
	protected override int AtlasIndex => 7 - 3;

	public class CardTopBottom : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer && canApplyParameters.WouldSufferDamage,
						async applyParameters =>
						{
							applyParameters.SetDamagePrevented();

							await state.ActionState.RequestDiscardOrLose();
						}, EffectType.Selectable);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}