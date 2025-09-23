using Fractural.Tasks;
using Godot;

public abstract class ConditionModel : AbstractModel<ConditionModel>, IEventSubscriber
{
	public abstract string Name { get; }
	public abstract string IconPath { get; }
	public virtual bool CanStack => false;
	public virtual bool CanBeUpgraded => false;
	public virtual ConditionModel ImmunityCompareBaseCondition => IsMutable ? ImmutableInstance : this;
	public virtual bool RemovedAtEndOfTurn => false;
	public virtual bool IsPositive => false;
	public virtual bool IsNegative => !IsPositive;
	public virtual bool RemovedByHeal => false;
	public virtual string ConditionAnimationScenePath => null;
	public virtual bool ShowOnFigure => true;

	private bool _appliedDuringThisTurn;

	protected Figure Owner { get; private set; }
	public ConditionNode Node { get; private set; }

	public virtual async GDTask Add(Figure target, ConditionNode node)
	{
		Owner = target;
		Node = node;

		if(target.TakingTurn)
		{
			_appliedDuringThisTurn = true;
		}

		ScenarioEvents.InflictConditionDuplicatesCheckEvent.Subscribe(this, DuplicatesCheckCanApply, DuplicatesCheckApply, EffectType.MandatoryBeforeOptionals);

		if(RemovedAtEndOfTurn)
		{
			ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.Subscribe(this, TurnEndedCanApply, TurnEndedApply, EffectType.MandatoryBeforeOptionals);
		}

		if(!GameController.FastForward && ConditionAnimationScenePath != null)
		{
			PackedScene conditionScene = ResourceLoader.Load<PackedScene>(ConditionAnimationScenePath);
			ConditionAnimation conditionAnimation = conditionScene.Instantiate<ConditionAnimation>();
			GameController.Instance.Map.AddChild(conditionAnimation);
			conditionAnimation.Init(target);

			await GDTask.Delay(0.5f);
		}
	}

	public virtual GDTask Remove()
	{
		ScenarioEvents.InflictConditionDuplicatesCheckEvent.Unsubscribe(this);
		ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.Unsubscribe(this);

		return GDTask.CompletedTask;
	}

	protected virtual bool DuplicatesCheckCanApply(ScenarioEvents.InflictConditionDuplicatesCheck.Parameters parameters)
	{
		return !parameters.Prevented && parameters.Target == Owner && parameters.Condition.ImmutableInstance == ImmutableInstance;
	}

	private GDTask DuplicatesCheckApply(ScenarioEvents.InflictConditionDuplicatesCheck.Parameters parameters)
	{
		parameters.SetPrevented(true);

		if(parameters.Target.TakingTurn)
		{
			_appliedDuringThisTurn = true;
		}

		return GDTask.CompletedTask;
	}

	private bool TurnEndedCanApply(ScenarioEvents.FigureTurnEndedConditionsFallOff.Parameters parameters)
	{
		return parameters.Figure == Owner;
	}

	private async GDTask TurnEndedApply(ScenarioEvents.FigureTurnEndedConditionsFallOff.Parameters parameters)
	{
		if(_appliedDuringThisTurn)
		{
			_appliedDuringThisTurn = false;
		}
		else
		{
			await AbilityCmd.RemoveCondition(Owner, ImmutableInstance);
		}
	}
}