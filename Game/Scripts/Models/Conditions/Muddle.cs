using Fractural.Tasks;

public class Muddle : ConditionModel
{
	public override string Name => "Muddle";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Muddle.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.AbilityState.SingleTargetSetHasDisadvantage();

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(condition,
			parameters => parameters.Attacker == condition.Owner,
			parameters => parameters.SetDisadvantage(true)
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(condition);
		ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(condition);
	}
}