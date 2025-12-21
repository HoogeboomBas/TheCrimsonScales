using Fractural.Tasks;

public class TargetsTrait(int targets) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState is AttackAbility.State &&
				parameters.AbilityState.Performer == figure, // && parameters.AbilityState.Targets < targets,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = ((AttackAbility.State)parameters.AbilityState);
				attackAbilityState.AdjustTargets(targets - 1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure,
			parameters =>
			{
				parameters.AdjustTargets(targets - 1);
			}
		);

		ScenarioCheckEvents.TargetsCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.AdjustTargets(targets - 1);
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.TargetsCheckEvent.Unsubscribe(figure, this);
	}
}