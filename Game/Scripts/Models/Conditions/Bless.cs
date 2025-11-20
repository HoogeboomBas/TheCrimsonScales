using Fractural.Tasks;

public class Bless : ConditionModel
{
	public override string Name => "Bless";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Bless.svg";
	public override bool CanStack => true;
	public override bool IsPositive => true;
	public override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/BlessAnimation.tscn";
	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		GameController.Instance.AMDManager.Bless(target);

		await AbilityCmd.RemoveCondition(target, this);
	}

	public override bool ShouldShowOnFigure(Figure figure)
    {
		return false;
    }
}