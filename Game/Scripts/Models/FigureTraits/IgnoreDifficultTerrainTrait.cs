public class IgnoreDifficultTerrainTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

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
				parameters.Add(new FigureInfoTextExtraEffect.Parameters(
					$"This figure ignores the effects of difficult terrain."));
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}