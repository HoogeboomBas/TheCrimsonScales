using Fractural.Tasks;

public class Regenerate : ConditionModel
{
	public override string Name => "Regenerate";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Regenerate.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool RemovedAtEndOfTurn => false;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(condition,
			parameters => parameters.Figure == condition.Owner,
			async parameters =>
			{
				condition.Flash();

				ActionState actionState = new ActionState(parameters.Figure,
					[
						HealAbility.Builder()
							.WithHealValue(1)
							.WithTarget(Target.Self)
							.Build()
					]
				);
				await actionState.Perform();
			},
			effectType: EffectType.MandatoryBeforeOptionals,
			order: -1
		);

		ScenarioEvents.AfterSufferDamageEvent.Subscribe(condition,
			canApply: parameters => parameters.Figure == condition.Owner,
			apply: async parameters =>
			{
				await AbilityCmd.RemoveCondition(condition);
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(condition);
		ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(condition);
	}
}