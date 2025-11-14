using System;
using System.Linq;
using Fractural.Tasks;

public abstract class Empower : ConditionModel
{
	public override string Name => "Empower";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Empower.svg";
	public override bool CanStack => true;
	public override bool IsPositive => true;
	public override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/EmpowerAnimation.tscn";
	public abstract Type CharacterType { get; }

	public override bool ShouldShowOnFigure(Figure figure)
	{
		return false;
	}
	
	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		IHasEmpower sourceFigure = (IHasEmpower)GameController.Instance.Map.Figures
			.FirstOrDefault(f => f.GetType() == CharacterType);

		if (sourceFigure != null)
		{
			GameController.Instance.AMDManager.Empower(sourceFigure, target);
		}

		await AbilityCmd.RemoveCondition(target, this);
	}
}