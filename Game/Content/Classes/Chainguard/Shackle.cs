using System.ComponentModel;
using System.Linq;
using Fractural.Tasks;

public class Shackle : ConditionModel
{
    public override string Name => "Shackle";
	public override string IconPath => "res://Content/Classes/Chainguard/Icon.svg";
	public override bool RemovedByHeal => false;
	public override bool CanBeUpgraded => false;
	public override ConditionModel BaseCondition => Conditions.Immobilize;

	protected Figure Cause;

	public void AddCause(Figure cause) 
	{
		Cause = cause;
	}

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		// Can only be applied to 1 figure
		ScenarioEvents.InflictConditionEvent.Subscribe(this,
			parameters => parameters.Condition is Shackle && parameters.Target != Owner,
			async parameters =>
			{
				await Owner.RemoveCondition(this);
			},
			EffectType.MandatoryBeforeOptionals
		);

		// Stop movement if became adjacent to the Chainguard
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters =>
				parameters.Figure == Owner &&
				parameters.AbilityState.Performer == Owner &&
				RangeHelper.GetFiguresInRange(parameters.Hex, 1).Any(figure => figure == Cause),
			async parameters =>
			{
				// If this was a MoveAbility and had movement left, make the figure stop
				ScenarioCheckEvents.CanMoveFurtherCheckEvent.Subscribe(this,
					canApplyParameters => canApplyParameters.Figure == Owner,
					async applyParameters =>
					{
						applyParameters.SetCannotMoveFurther();
						await GDTask.CompletedTask;
					}
				);

				// MoveAbility had no more movement, cancel when ability ends
				ScenarioEvents.AbilityEndedEvent.Subscribe(this,
					canApplyParameters => canApplyParameters.AbilityState == parameters.AbilityState,
					async applyParameters =>
					{
						ScenarioCheckEvents.CanMoveFurtherCheckEvent.Unsubscribe(this);
						ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
						await GDTask.CompletedTask;
					}
				);
				
				await GDTask.CompletedTask;
			});

		// Don't allow new movement when adjacent to the Chainguard
		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.Performer == Owner && parameters.AbilityState is MoveAbility.State &&
			RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == Cause),
			parameters =>
			{
				Node.Flash();
				parameters.SetIsBlocked(true);
				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.InflictConditionEvent.Unsubscribe(this);
		ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this);
		ScenarioCheckEvents.CanMoveFurtherCheckEvent.Unsubscribe(this);
		ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
	}
}
