using Fractural.Tasks;

public class Wound : ConditionModel
{
	public override string Name => "Wound";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Wound.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Wound1];
	public override ConditionModel BaseLevelCondition => Conditions.Wound1;
	public override int UpgradableLevel => 1;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(condition,
			parameters => parameters.Figure == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				await AbilityCmd.SufferDamage(condition.Owner, UpgradableLevel, condition.Owner);
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(condition);
	}
}