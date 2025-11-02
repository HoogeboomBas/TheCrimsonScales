public class FlyingTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters => parameters.SetFlying(true));

		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Unsubscribe(figure, this);
	}
}