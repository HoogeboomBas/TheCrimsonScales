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
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(figure, this);
	}
}