using Fractural.Tasks;

public abstract class ConditionModel : AbstractModel
{
	private Tooltip _tooltip;

	public abstract string Name { get; }
	public abstract string IconPath { get; }

	public abstract ConditionPolarity ConditionPolarity { get; }
	public virtual bool CanBeAppliedMultipleTimesOnSingleTarget => false;
	public virtual ConditionModel[] ImmunityCompareBaseConditions => [this];
	public virtual bool RemovedAtEndOfTurn => false;
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

	public Tooltip Tooltip
	{
		get
		{
			if(_tooltip == null)
			{
				_tooltip = new Tooltip(Name, IconPath, $"{ConditionPolarity.ToString()} Condition", GetTooltipText);
			}

			return _tooltip;
		}
	}

	public virtual async GDTask OnAdded(Condition condition)
	{
		if(!GameController.FastForward && ConditionAnimationScenePath != null)
		{
			ConditionAnimation conditionAnimation = SceneLoader.InstantiateScene<ConditionAnimation>(ConditionAnimationScenePath);
			GameController.Instance.Map.AddChild(conditionAnimation);
			conditionAnimation.Init(condition.Owner);

			await GDTask.DelayFastForwardable(0.5f);
		}
	}

	public virtual async GDTask OnRemoved(Condition condition)
	{
		await GDTask.CompletedTask;
	}

	protected abstract string GetTooltipText(RichTextParameters textParameters);
}