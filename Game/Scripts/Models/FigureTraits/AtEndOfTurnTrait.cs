using System;
using Fractural.Tasks;

public class AtEndOfTurnTrait(Func<Figure, GDTask> endOfTurn, string endOfTurnDescription) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			async parameters =>
			{
				await endOfTurn(figure);
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters($"{endOfTurnDescription} at end of turn"));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}