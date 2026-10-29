using Fractural.Tasks;

public class Brittle : ConditionModel
{
	public override string Name => "Brittle";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Brittle.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.SufferDamageEvent.Subscribe(condition,
			parameters => parameters.Figure == condition.Owner && parameters.WouldSufferDamage,
			async parameters =>
			{
				condition.Flash();
				parameters.SetBrittle(true);

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