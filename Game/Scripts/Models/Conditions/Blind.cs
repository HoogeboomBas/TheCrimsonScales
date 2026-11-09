using Fractural.Tasks;

public class Blind : ConditionModel
{
	public override string Name => "Blind";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Blind.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.DuringAttackEvent.Subscribe(condition,
			parameters => parameters.Performer == condition.Owner &&
			parameters.AbilityState.AbilityRangeType == RangeType.Range,
			async parameters =>
			{
				condition.Flash();
				parameters.AbilityState.SingleTargetRange = 1;

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.DuringAttackEvent.Unsubscribe(condition);
	}
}