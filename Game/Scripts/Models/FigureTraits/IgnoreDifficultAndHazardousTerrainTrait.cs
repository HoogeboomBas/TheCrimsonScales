using Fractural.Tasks;

public class IgnoreDifficultAndHazardousTerrainTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(figure, this,
			canApplyParameters =>
				canApplyParameters.Performer == figure &&
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

		ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(figure, this,
			canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == figure,
			async applyParameters =>
			{
				applyParameters.SetAffectedByHazardousTerrain(false);
				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(figure, this);
	}
}