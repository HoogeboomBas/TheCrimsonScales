using Fractural.Tasks;

public class Poison : ConditionModel
{
	public override string Name => "Poison";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Poison.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Poison1];
	public override ConditionModel BaseLevelCondition => Conditions.Poison1;
	public override int UpgradableLevel => 1;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(condition,
			parameters => parameters.AbilityState.Target == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.AbilityState.SingleTargetAdjustAttackValue(UpgradableLevel);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.HealBlockTimeEvent.Subscribe(condition,
			parameters => parameters.AbilityState.Target == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.SetBlocked(true);

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(condition);
		ScenarioEvents.HealBlockTimeEvent.Unsubscribe(condition);
	}
}