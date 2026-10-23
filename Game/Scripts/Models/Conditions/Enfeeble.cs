using Fractural.Tasks;

public abstract class Enfeeble : ConditionModel
{
	public override string Name => "Enfeeble";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Enfeeble.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override bool ImmediatelyRemovedOnApply => true;
	public override bool RequiresGiver => true;
	public override bool ShouldShowOnFigure => false;
	protected override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/EnfeebleAnimation.tscn";

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		//TODO: Currently expects the giver or its summoner to have enfeebles, should probably retrace to original owner of the ability card used to perform the ability?
		IHasEnfeeble hasEnfeeble;
		if(condition.PotentialGiver is Summon summon)
		{
			hasEnfeeble = summon.CharacterOwner as IHasEnfeeble;
		}
		else
		{
			hasEnfeeble = condition.PotentialGiver as IHasEnfeeble;
		}

		if(hasEnfeeble != null)
		{
			await GameController.Instance.AMDManager.Enfeeble(hasEnfeeble, condition.Owner);
		}
	}
}