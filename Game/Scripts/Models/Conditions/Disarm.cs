using Fractural.Tasks;

public class Disarm : ConditionModel
{
	public override string Name => "Disarm";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Disarm.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				parameters.AbilityState is AttackAbility.State,
			async parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);
	}
}