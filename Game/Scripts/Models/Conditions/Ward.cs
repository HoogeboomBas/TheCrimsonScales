using Fractural.Tasks;

public class Ward : ConditionModel
{
	public override string Name => "Ward";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Ward.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.SufferDamageEvent.Subscribe(condition,
			parameters => parameters.Figure == condition.Owner,
			async parameters =>
			{
				condition.Flash();
				parameters.SetWard(true);

				await AbilityCmd.RemoveCondition(condition, parameters.PotentialAbilityState);
			},
			EffectType.MandatoryBeforeOptionals, 100);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.SufferDamageEvent.Unsubscribe(condition);
	}
}