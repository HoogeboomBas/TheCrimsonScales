using Fractural.Tasks;

public class ApplyConditionTrait(ConditionModel conditionModel) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAddCondition(conditionModel);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AppliesVisualCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.AddConditionModel(conditionModel);
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.AppliesVisualCheckEvent.Unsubscribe(figure, this);
	}
}