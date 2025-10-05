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
	public override ConditionModel ImmunityCompareBaseCondition => Conditions.Immobilize;

	public Figure Shackler;

	public void AddShackler(Figure shackler) 
	{
		Shackler = shackler;
	}

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		// Stop movement if became adjacent to the Shackler
		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(target, this, 
			parameters => parameters.Performer == Owner && 
				RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == Shackler),
			async parameters =>
			{
				Node.Flash();
				parameters.SetCannotMoveFurther(true);

				await GDTask.CompletedTask;
			}
		);

		// Don't allow new movement when adjacent to the Shackler
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

		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(Owner, this);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(Owner, this);
	}
}
