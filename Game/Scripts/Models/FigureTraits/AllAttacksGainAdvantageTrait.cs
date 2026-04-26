using Fractural.Tasks;

public class AllAttacksGainAdvantageTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(figure, this,
			parameters => parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetSetHasAdvantage();

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "All attacks by this figure gain advantage."));
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}