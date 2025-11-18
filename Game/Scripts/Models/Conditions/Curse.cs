using Fractural.Tasks;

public class Curse : ConditionModel
{
	public override string Name => "Curse";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Curse.svg";
	public override bool CanStack => true;
	public override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/CurseAnimation.tscn";

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		GameController.Instance.AMDManager.Curse(target);

		await AbilityCmd.RemoveCondition(target, this);
	}

	public override bool ShouldShowOnFigure(Figure figure)
    {
		return false;
    }
}