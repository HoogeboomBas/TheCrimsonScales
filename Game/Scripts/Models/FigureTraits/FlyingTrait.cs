public class FlyingTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters => parameters.SetFlying(true));

		//figure.UpdateFlying();
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(figure, this);
	}
}