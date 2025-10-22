using Fractural.Tasks;
using System.Collections.Generic;
using Godot;

public class Chill : ConditionModel
{
	public override string Name => "Chill";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Chill.svg";
	public override bool CanBeUpgraded => true;
	public override bool RemovedAtEndOfTurn => true;
	public override ConditionModel[] ImmunityCompareBaseCondition => [Conditions.Immobilize, Conditions.Muddle];

	int _chillStacks = 1;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		Chill existingChill = target.GetCondition(global::Conditions.Chill) as Chill;
		if (existingChill != null && existingChill != this)
		{
			existingChill.ChangeChillStacks(1);
			node.Destroy();
		}
		else
		{
			await base.Add(target, node);
			_appliedDuringThisTurn = false;
			SubscribeToChill();
		}
	}
	public override GDTask Remove()
	{
		if (_chillStacks > 1)
		{
			ChangeChillStacks(-1);
		}
		else
		{
			ConditionNode node = this.Node;
			node?.Destroy();
			Owner.Conditions.Remove(this);
			ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
			ScenarioEvents.InflictConditionDuplicatesCheckEvent.Unsubscribe(this);
			ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.Unsubscribe(this);
		}
	
		return GDTask.CompletedTask;
	}
	
	private void ChangeChillStacks(int change)
	{
		_chillStacks += change;
		GD.Print(_chillStacks);
		Node.SetStackText(_chillStacks == 1 ? null : _chillStacks.ToString());
	}
	
	protected override GDTask DuplicatesCheckApply(ScenarioEvents.InflictConditionDuplicatesCheck.Parameters parameters)
		{
			parameters.SetPrevented(true);
	
			return GDTask.CompletedTask;
		}

	protected override bool DuplicatesCheckCanApply(ScenarioEvents.InflictConditionDuplicatesCheck.Parameters parameters)
	{
		if (parameters.Condition is Chill)
		{
			return false;
		}
		return base.DuplicatesCheckCanApply(parameters);
	}
	
	private void SubscribeToChill()
	{
		ScenarioEvents.AbilityStartedEvent.Subscribe(
			this,
			parameters => parameters.Performer == Owner 
						  && (parameters.AbilityState is AttackAbility.State || parameters.AbilityState is MoveAbility.State),
			parameters =>
			{
				Chill chill = Owner.GetCondition(global::Conditions.Chill) as Chill;
				int currentStacks = chill?._chillStacks ?? 1;
		
				switch (parameters.AbilityState)
				{
					case AttackAbility.State attackState:
						attackState.AbilityAdjustAttackValue(-currentStacks);
						break;
		
					case MoveAbility.State moveState:
						moveState.AdjustMoveValue(-currentStacks);
						break;
				}
		
				Node.Flash();
				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals
		);
	}
}
