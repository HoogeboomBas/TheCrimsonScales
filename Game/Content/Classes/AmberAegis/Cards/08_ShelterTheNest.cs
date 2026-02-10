using System.Collections.Generic;
using Fractural.Tasks;

public class ShelterTheNest : AmberAegisCardModel<ShelterTheNest.CardTop, ShelterTheNest.CardBottom>
{
	public override string Name => "Shelter the Nest";
	public override int Level => 1;
	public override int Initiative => 16;
	protected override int AtlasIndex => 8;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PlaceColonyTokenAbility<RockspineTermiteColony>([Element.Earth])),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => IsAdjacentToColonyToken<RockspineTermiteColony>(parameters.Figure) &&
						              parameters.Figure.AlliedWith(state.Performer, true) && parameters.FromAttack,
						async parameters =>
						{
							parameters.AdjustShield(1);
							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure, true) &&
							IsAdjacentToColonyToken<RockspineTermiteColony>(parameters.Figure),
						applyParameters =>
						{
							applyParameters.AdjustShield(1);
						}
					);

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						async parameters =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						},
						EffectType.Visuals
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override string CustomTag => "Cultivate";
		public override bool Persistent => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build())
		];
	}
}