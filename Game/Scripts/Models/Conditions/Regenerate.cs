using Fractural.Tasks;

public class Regenerate : ConditionModel
{
	public override string Name => "Regenerate";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Regenerate.svg";
	public override bool IsPositive => true;
	public override bool RemovedAtEndOfTurn => false;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(Owner, this,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				Node.Flash();
				HealAbility heal = HealAbility.Builder()
					.WithHealValue(1)
					.WithTarget(Target.Self)
					.WithCanPerformWhileStunned(true)
					.Build();
				ActionState actionState = new ActionState(parameters.Figure, [heal]);
				await actionState.Perform();
			},
			effectType: EffectType.MandatoryBeforeOptionals,
			order: -1
		);

		ScenarioEvents.AfterSufferDamageEvent.Subscribe(Owner, this,
			canApply: parameters => parameters.Figure == Owner,
			apply: async parameters =>
			{
				await AbilityCmd.RemoveCondition(target, this);
				await Remove();
			});
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(Owner, this);
		ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(Owner, this);
	}
}