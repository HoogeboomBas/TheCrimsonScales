using Fractural.Tasks;

public class IgnoreDifficultTerrainTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(figure, this,
			canApplyParameters =>
				canApplyParameters.Performer == figure &&
				canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
			applyParameters =>
			{
				applyParameters.SetMoveCost(1);
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"This figure ignores the effects of difficult terrain."));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}