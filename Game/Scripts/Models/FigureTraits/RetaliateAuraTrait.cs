using Fractural.Tasks;

public class RetaliateAuraTrait(int retaliate, int range, bool canBeSelf) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(figure, this,
			parameters =>
				figure.AlliedWith(parameters.Figure, canBeSelf) &&
				RangeHelper.Distance(parameters.Figure.Hex, figure.Hex) <= range,
			applyParameters =>
			{
				applyParameters.AddRetaliate(retaliate, range);
			}
		);

		ScenarioEvents.RetaliateEvent.Subscribe(figure, this,
			canApplyParameters =>
				canApplyParameters.RetaliatingFigure == figure,
			async applyParameters =>
			{
				applyParameters.AddRetaliate(retaliate, range);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => figure.AlliedWith(parameters.Figure, true),
			async parameters =>
			{
				ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.RetaliateEvent.Unsubscribe(figure, this);
		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(figure, this);
	}
}