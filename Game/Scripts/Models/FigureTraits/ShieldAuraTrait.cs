using Fractural.Tasks;

public class ShieldAuraTrait(int shield, int range, bool canBeSelf) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(figure, this,
			parameters =>
				figure.AlliedWith(parameters.Figure, canBeSelf) &&
				RangeHelper.Distance(parameters.Figure.Hex, figure.Hex) <= range,
			applyParameters =>
			{
				applyParameters.AdjustShield(shield);
			}
		);

		ScenarioEvents.SufferDamageEvent.Subscribe(figure, this,
			parameters =>
				figure.AlliedWith(parameters.Figure, canBeSelf) &&
				RangeHelper.Distance(parameters.Figure.Hex, figure.Hex) <= range,
			async parameters =>
			{
				parameters.AdjustShield(shield);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => figure.AlliedWith(parameters.Figure, true),
			async parameters =>
			{
				ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, this);
		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(figure, this);
	}
}