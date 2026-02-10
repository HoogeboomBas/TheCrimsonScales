using System;
using Fractural.Tasks;
using Godot;

public abstract class ConditionModel : AbstractModel
{
	public abstract string Name { get; }
	public abstract string IconPath { get; }
	public abstract ConditionPolarity ConditionPolarity { get; }
	public virtual bool CanBeAppliedMultipleTimesOnSingleTarget => false;
	public virtual ConditionModel[] ImmunityCompareBaseConditions => [this];
	public virtual bool RemovedAtEndOfTurn => false;
	public virtual Func<Condition, GDTask> OnRemovedAtEndOfTurn => null;
	public virtual bool ImmediatelyRemovedOnApply => false;
	public virtual bool RemovedByHeal => false;
	public virtual ConditionModel BaseLevelCondition => this;
	public virtual int UpgradableLevel => 1;
	public virtual bool RequiresGiver => false;
	public virtual bool Stackable => false;
	public virtual bool ShouldShowOnFigure => true;
	protected virtual string ConditionAnimationScenePath => null;

	public bool IsPositive => ConditionPolarity == ConditionPolarity.Positive;
	public bool IsNegative => ConditionPolarity == ConditionPolarity.Negative;

	public virtual async GDTask OnAdded(Condition condition)
	{
		if(!GameController.FastForward && ConditionAnimationScenePath != null)
		{
			PackedScene conditionScene = ResourceLoader.Load<PackedScene>(ConditionAnimationScenePath);
			ConditionAnimation conditionAnimation = conditionScene.Instantiate<ConditionAnimation>();
			GameController.Instance.Map.AddChild(conditionAnimation);
			conditionAnimation.Init(condition.Owner);

			await GDTask.Delay(0.5f);
		}
	}

	public virtual async GDTask OnRemoved(Condition condition)
	{
		await GDTask.CompletedTask;
	}
}