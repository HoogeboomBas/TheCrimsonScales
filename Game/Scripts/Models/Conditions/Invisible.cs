using Fractural.Tasks;

public class Invisible : ConditionModel
{
	public override string Name => "Invisible";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Invisible.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(condition,
			parameters =>
				parameters.PotentialTarget == condition.Owner &&
				!ScenarioCheckEvents.CanTargetInvisibleCheckEvent.Fire(
					new ScenarioCheckEvents.CanTargetInvisibleCheck.Parameters(parameters.Performer)).CanTargetInvisible &&
				parameters.Performer.EnemiesWith(condition.Owner),
			parameters =>
			{
				parameters.SetCannotBeFocused();
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(condition,
			parameters =>
				parameters.PotentialTarget == condition.Owner &&
				!ScenarioCheckEvents.CanTargetInvisibleCheckEvent.Fire(
					new ScenarioCheckEvents.CanTargetInvisibleCheck.Parameters(parameters.Performer)).CanTargetInvisible &&
				parameters.Performer.EnemiesWith(condition.Owner),
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(condition,
			parameters => parameters.EnemyFigure == condition.Owner &&
			              !ScenarioCheckEvents.CanTargetInvisibleCheckEvent
				              .Fire(new ScenarioCheckEvents.CanTargetInvisibleCheck.Parameters(parameters.Figure)).CanTargetInvisible,
			parameters =>
			{
				parameters.SetCanPass();
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(condition);
	}
}