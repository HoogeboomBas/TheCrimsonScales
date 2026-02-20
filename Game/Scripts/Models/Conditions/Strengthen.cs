using Fractural.Tasks;

public class Strengthen : ConditionModel
{
	public override string Name => "Strengthen";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Strengthen.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.AbilityState.SingleTargetSetHasAdvantage();

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(condition);
	}
}