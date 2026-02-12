using Fractural.Tasks;

public class FlyingTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters => parameters.SetFlying(true));
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(figure, this);
	}
}