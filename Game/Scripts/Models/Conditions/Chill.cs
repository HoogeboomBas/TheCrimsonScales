using Fractural.Tasks;

public class Chill : ConditionModel
{
	public override string Name => "Chill";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Chill.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Immobilize, Conditions.Muddle];
	public override bool RemovedAtEndOfTurn => true;
	public override bool Stackable => true;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				parameters.AbilityState is AttackAbility.State or MoveAbility.State,
			parameters =>
			{
				if(parameters.AbilityState is AttackAbility.State attackState)
				{
					attackState.AbilityAdjustAttackValue(-condition.StackCount);
				}
				else if(parameters.AbilityState is MoveAbility.State moveState)
				{
					moveState.AdjustMoveValue(-condition.StackCount);
				}

				condition.Flash();

				return GDTask.CompletedTask;
			}
		);
	}

	public override GDTask OnRemoved(Condition condition)
	{
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);

		return GDTask.CompletedTask;
	}
}