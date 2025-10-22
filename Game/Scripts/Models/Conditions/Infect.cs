using Fractural.Tasks;

public class Infect : ConditionModel
{
	public override string Name => "Infect";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Infect.svg";
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseCondition => [Conditions.Poison1];

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);
		
		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
					parameters => parameters.Performer == Owner && parameters.AbilityState is ShieldAbility.State,
					parameters =>
					{
						Node.Flash();
						parameters.SetIsBlocked(true);
						return GDTask.CompletedTask;
					},
					EffectType.MandatoryBeforeOptionals);
					
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this, CanApply, Apply, EffectType.MandatoryBeforeOptionals);

		ScenarioEvents.HealBlockTimeEvent.Subscribe(this,
					parameters => parameters.AbilityState.Target == Owner,
					parameters =>
					{
						Node.Flash();
						parameters.SetBlocked(true);
						return GDTask.CompletedTask;
					},
					EffectType.MandatoryBeforeOptionals
				);

	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(this);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
		ScenarioEvents.HealBlockTimeEvent.Unsubscribe(this);
	}
	
	private bool CanApply(ScenarioEvents.AttackAfterTargetConfirmed.Parameters abilityStateParameters)
	{
		return abilityStateParameters.AbilityState.Target == Owner;
	}
	
	private GDTask Apply(ScenarioEvents.AttackAfterTargetConfirmed.Parameters abilityStateParameters)
	{
		Node.Flash();
		abilityStateParameters.AbilityState.SingleTargetSetIgnoresAllShields();
		return GDTask.CompletedTask;
	}
}
