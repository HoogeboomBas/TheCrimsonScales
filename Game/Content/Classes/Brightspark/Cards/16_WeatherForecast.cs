using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WeatherForecast : BrightsparkCardModel<WeatherForecast.CardTop, WeatherForecast.CardBottom>
{
	public override string Name => "Weather Forecast";
	public override int Level => 3;
	public override int Initiative => 30;
	protected override int AtlasIndex => 16;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5f, 0.24867724f)))
				.WithRange(3)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							Element element = state.UseSlotIndex switch
							{
								0 => Element.Ice,
								1 => Element.Light,
								2 => Element.Air,
								_ => throw new ArgumentOutOfRangeException()
							};
							await AbilityCmd.InfuseElement(state, element);
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29185185f, 0.81824744f)),
					new UseSlot(new Vector2(0.49777776f, 0.81824744f)),
					new UseSlot(new Vector2(0.7074074f, 0.81824744f), GainXP)
				])
				.Build()),
		];

		public override bool Persistent => true;
	}
}