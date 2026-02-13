using Fractural.Tasks;

public class Immobilize : ConditionModel
{
	public override string Name => "Immobilize";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Immobilize.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				parameters.AbilityState is MoveAbility.State,
			parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				return GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner,
			parameters =>
			{
				condition.Flash();
				parameters.SetCannotMoveFurther(true);

				return GDTask.CompletedTask;
			},
			order: 100
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);
		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(condition);
	}
}