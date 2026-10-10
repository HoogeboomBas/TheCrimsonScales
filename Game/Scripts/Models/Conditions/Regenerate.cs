using System;
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
			canApply: parameters => parameters.Figure == condition.Owner && parameters.DamageSuffered > 0,
			apply: async parameters =>
			{
				await AbilityCmd.RemoveCondition(condition, parameters.PotentialAbilityState);
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(condition);
		ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(condition);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		$"The affected figure performs “{Icons.Inline(Icons.Heal, textParameters)}1, self” at the start of each of their turns, even if they have {Icons.InlineCondition(Conditions.Stun, textParameters)}. This condition is removed when the figure suffers {Icons.Inline(Icons.Damage, textParameters)}. If a figure also has {Icons.InlineCondition(Conditions.Wound1, textParameters)}, this effect is applied first, removing {Icons.InlineCondition(Conditions.Wound1, textParameters)} before it takes effect.";
}