using System;
using Fractural.Tasks;

public class Bane : ConditionModel
{
	public override string Name => "Bane";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Bane.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool RemovedAtEndOfTurn => true;
	public override bool RemovedByHeal => true;

	public override Func<Condition, GDTask> OnRemovedAtEndOfTurn =>
		async condition => await AbilityCmd.SufferDamage(condition.Owner, 10, condition.Owner);
}