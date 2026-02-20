using Fractural.Tasks;

public class Infect : ConditionModel
{
	public override string Name => "Infect";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Infect.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Poison1];

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		condition.Owner.SetCrackedShield(true);

		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				parameters.AbilityState is ShieldAbility.State,
			async parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(condition,
			parameters => parameters.AbilityState.Target == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.AbilityState.SingleTargetSetIgnoresAllShields();

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		condition.Owner.SetCrackedShield(false);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(condition);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);
	}
}