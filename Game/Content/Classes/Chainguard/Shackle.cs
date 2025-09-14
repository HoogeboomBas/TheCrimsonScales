using System.ComponentModel;
using System.Linq;
using Fractural.Tasks;

public class Shackle : ConditionModel
{
    public override string Name => "Shackle";
	public override string IconPath => "res://Content/Classes/Chainguard/Icon.svg";
	public override bool RemovedByHeal => false;
	public override bool CanBeUpgraded => false;
	public override bool IsPositive => false;
	public override bool IsNegative => false;
	public override ConditionModel BaseCondition => Conditions.Immobilize;

	public Figure Shackler;

	public void AddShackler(Figure shackler) 
	{
		Shackler = shackler;
	}

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		// Can only be applied to 1 figure
		ScenarioEvents.InflictConditionEvent.Subscribe(target, this,
			parameters => parameters.Condition is Shackle && parameters.Target != Owner,
			async parameters =>
			{
				await AbilityCmd.RemoveCondition(Owner, this);
			},
			EffectType.MandatoryBeforeOptionals
		);

		// Stop movement if became adjacent to the Chainguard
		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(target, this, 
			parameters => parameters.Performer == Owner && 
				RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == Shackler),
			async parameters =>
			{
				Node.Flash();
				parameters.SetCannotMoveFurther();

				await GDTask.CompletedTask;
			}
		);

		// Don't allow new movement when adjacent to the Chainguard
		ScenarioEvents.AbilityStartedEvent.Subscribe(target, this,
			parameters => parameters.Performer == Owner && parameters.AbilityState is MoveAbility.State &&
				RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == Shackler),
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

		ScenarioEvents.InflictConditionEvent.Unsubscribe(Owner, this);
		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(Owner, this);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(Owner, this);
	}
}
