using System;
using System.Linq;
using Fractural.Tasks;

public class Rupture : ConditionModel
{
	public override string Name => "Rupture";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Rupture.svg";
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Wound1];
	public override bool IsNegative => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);
		
		ScenarioEvents.AbilityEndedEvent.Subscribe(this,
			//TODO: Make rupture trigger at the end of the movement rather than the end of the ability (matters on some rare occasions)
			parameters =>
				parameters.AbilityState is TargetedAbilityState<SingleTargetState> targetedAbilityState &&
				((targetedAbilityState.SingleTargetStates
					.FirstOrDefault(singleTargetState => singleTargetState.Target == target)
					?.ForcedMovementHexes.Count ?? 0) > 0) ||
				(parameters.AbilityState.Performer == target && parameters.AbilityState is MoveAbility.State moveState && moveState.Hexes.Count > 0),
			async parameters =>
			{
				await AbilityCmd.SufferDamage(parameters.AbilityState, target, 1);
			},
			EffectType.MandatoryBeforeOptionals);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
	}
}
