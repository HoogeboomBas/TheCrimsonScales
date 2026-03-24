using Fractural.Tasks;

public class Stun : ConditionModel
{
	public override string Name => "Stun";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Stun.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				!parameters.AbilityState.CanPerformWhileStunned,
			async parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.SetCannotMoveFurther();

				await GDTask.CompletedTask;
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