using Fractural.Tasks;

public class IgnoreHazardousTerrainTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(figure, this,
			canApplyParameters =>
				canApplyParameters.Performer == figure &&
				canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>(),
			applyParameters =>
			{
				applyParameters.SetAffectedByNegativeHex(false);
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

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new FigureInfoTextExtraEffect.Parameters(
					$"This figure ignores the effects of hazardous terrain."));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}