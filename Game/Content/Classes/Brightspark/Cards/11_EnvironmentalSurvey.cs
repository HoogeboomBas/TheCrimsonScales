using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class EnvironmentalSurvey : BrightsparkCardModel<EnvironmentalSurvey.CardTop, EnvironmentalSurvey.CardBottom>
{
	public override string Name => "Environmental Survey";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 11;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.RelocateOverlayTile(state, overlayTiles =>
						{
							overlayTiles.AddRange(RangeHelper.GetOverlayTilesInRange<Obstacle>(state.Performer, 2)
								.Where(obstacle => obstacle.HexObjectShape is HexObjectShape.Single && !obstacle.CannotBeMoved));
							overlayTiles.AddRange(RangeHelper.GetOverlayTilesInRange<Trap>(state.Performer, 2).Where(trap => !trap.CannotBeMoved));
						},
						(_, hexes) => hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 2).Where(moveToHex => moveToHex.IsEmpty())),
						"Select a trap or obstacle to relocate");
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override int XP => 1;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer.AlliedWith(state.Performer, true) &&
							RangeHelper.Distance(canApplyParameters.Hex, state.Performer.Hex) <= 4 &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() ||
							 canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}

							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						}
					);

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.AlliedWith(state.Performer, true) &&
						                      RangeHelper.Distance(canApplyParameters.Hex, state.Performer.Hex) <= 4,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}