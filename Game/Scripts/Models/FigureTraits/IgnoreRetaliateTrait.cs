using Fractural.Tasks;

public class IgnoreRetaliateTrait : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.RetaliateEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure,
			async parameters =>
			{
				parameters.SetRetaliateBlocked();

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters($"When attacking, ignore {Icons.Inline(Icons.Retaliate)}"));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.RetaliateEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}