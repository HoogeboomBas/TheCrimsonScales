using Fractural.Tasks;

public class Curse : ConditionModel
{
	public override string Name => "Curse";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Curse.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override bool ImmediatelyRemovedOnApply => true;
	public override bool ShouldShowOnFigure => false;
	protected override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/CurseAnimation.tscn";

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		GameController.Instance.AMDManager.Curse(condition.Owner);
	}
}