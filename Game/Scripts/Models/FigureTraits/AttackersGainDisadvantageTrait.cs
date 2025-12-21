using Fractural.Tasks;

public class AttackersGainDisadvantageTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(figure, this,
			parameters => parameters.AbilityState.Target == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetSetHasDisadvantage();

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters("Attackers gain disadvantage on all their attacks targeting this figure."));
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