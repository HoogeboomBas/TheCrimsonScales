using Fractural.Tasks;

public class Bless : ConditionModel
{
	public override string Name => "Bless";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Bless.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override bool ImmediatelyRemovedOnApply => true;
	public override bool ShouldShowOnFigure => false;
	protected override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/BlessAnimation.tscn";

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		GameController.Instance.AMDManager.Bless(condition.Owner);
	}
}