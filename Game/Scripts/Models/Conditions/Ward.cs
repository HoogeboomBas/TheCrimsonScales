using System;
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
			parameters => parameters.Figure == condition.Owner && parameters.WouldSufferDamage,
			async parameters =>
			{
				if(parameters.CalculatedCurrentDamage > 0)
				{
					parameters.SetWard(true);
					condition.Flash();

					await AbilityCmd.RemoveCondition(condition, parameters.PotentialAbilityState);
				}
			},
			EffectType.MandatoryAfterOptionals, 100);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.SufferDamageEvent.Unsubscribe(condition);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		"The next time the affected figure suffers damage from any source, they instead suffer half that amount of damage (rounded down), and this condition is then removed.";
}