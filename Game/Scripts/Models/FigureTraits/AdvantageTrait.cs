using Fractural.Tasks;

public class AdvantageTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

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
				parameters.Add(new FigureInfoTextExtraEffect.Parameters("This figure gains advantage on all their attacks."));
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(figure, this);
	}
}