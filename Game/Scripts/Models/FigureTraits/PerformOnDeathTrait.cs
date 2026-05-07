using Fractural.Tasks;

public class PerformOnDeathTrait(params Ability[] abilities) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.BeforeFigureKilledEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			async parameters =>
			{
				ActionState actionState = new ActionState(figure, abilities);
				await actionState.Perform();
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => $"Performs abilities on death"));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}