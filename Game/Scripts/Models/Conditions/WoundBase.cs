using Fractural.Tasks;

public abstract class WoundBase : ConditionModel
{
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Wound.svg";
	public override bool RemovedByHeal => true;
	public override bool CanBeUpgraded => true;
	public override ConditionModel[] ImmunityCompareBaseCondition => [Conditions.Wound1];

	protected abstract int WoundValue { get; }

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		for(int i = target.Conditions.Count - 1; i >= 0; i--)
		{
			ConditionModel condition = target.Conditions[i];
			if(condition is WoundBase && condition != this)
			{
				await AbilityCmd.RemoveCondition(target, condition);
			}
		}

		Node.SetStackText(WoundValue == 1 ? null : WoundValue.ToString());

		ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				Node.Flash();
				await AbilityCmd.SufferDamage(null, Owner, WoundValue);
			},
			EffectType.MandatoryBeforeOptionals
		);

		// ScenarioEvents.AfterHealPerformedEvent.Subscribe(this,
		// 	parameters => parameters.AbilityState.Target == Owner,
		// 	async parameters => await AbilityCmd.RemoveCondition(Owner, this), EffectType.MandatoryBeforeOptionals);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(this);
		//ScenarioEvents.AfterHealPerformedEvent.Unsubscribe(this);
	}

	protected override bool DuplicatesCheckCanApply(ScenarioEvents.InflictConditionDuplicatesCheck.Parameters parameters)
	{
		return
			base.DuplicatesCheckCanApply(parameters) ||
			(!parameters.Prevented && parameters.Target == Owner && parameters.Condition.ImmutableInstance is WoundBase woundBase && woundBase.WoundValue < WoundValue);
	}
}