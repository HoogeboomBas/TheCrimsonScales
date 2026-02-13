using System.Linq;
using Fractural.Tasks;

public class Rupture : ConditionModel
{
	public override string Name => "Rupture";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Rupture.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedByHeal => true;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Wound1];

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.AbilityEndedEvent.Subscribe(condition,
			//TODO: Make rupture trigger at the end of the movement rather than the end of the ability (matters on some rare occasions)
			parameters =>
				parameters.AbilityState is TargetedAbilityState<SingleTargetState> targetedAbilityState &&
				((targetedAbilityState.SingleTargetStates
					.FirstOrDefault(singleTargetState => singleTargetState.Target == condition.Owner)
					?.ForcedMovementHexes.Count ?? 0) > 0) ||
				(parameters.AbilityState.Performer == condition.Owner &&
				 parameters.AbilityState is MoveAbility.State moveState && moveState.Hexes.Count > 0),
			async parameters =>
			{
				await AbilityCmd.SufferDamage(parameters.AbilityState, condition.Owner, 1);
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.AbilityEndedEvent.Unsubscribe(condition);
	}
}