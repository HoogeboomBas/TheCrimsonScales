using System.Linq;
using Fractural.Tasks;
using Godot;

public class Shackle : ConditionModel
{
	public override string Name => "Shackle";
	public override string IconPath => "res://Content/Classes/Chainguard/Shackle.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override ConditionModel[] ImmunityCompareBaseConditions => [Conditions.Immobilize];
	public override bool RequiresGiver => true;
	public override bool ShouldShowOnFigure => false;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ShackleIndicator indicator =
			ResourceLoader.Load<PackedScene>("res://Content/Classes/Chainguard/ShackleIndicator.tscn").Instantiate<ShackleIndicator>();
		condition.Owner.AddChild(indicator);
		indicator.Init();
		condition.SetCustomValue("ShackleIndicator", indicator);

		// Stop movement if became adjacent to the Shackler
		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner &&
				RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == condition.PotentialGiver),
			async parameters =>
			{
				condition.Flash();
				parameters.SetCannotMoveFurther(true);

				await GDTask.CompletedTask;
			}
		);

		// Don't allow new movement when adjacent to the Shackler
		ScenarioEvents.AbilityStartedEvent.Subscribe(condition,
			parameters =>
				parameters.Performer == condition.Owner && parameters.AbilityState is MoveAbility.State &&
				RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == condition.PotentialGiver),
			parameters =>
			{
				condition.Flash();
				parameters.SetIsBlocked(true);

				return GDTask.CompletedTask;
			}
		);

		// Don't allow movement through an ally that is adjacent to the Chainguard
		ScenarioCheckEvents.CanPassAllyCheckEvent.Subscribe(condition,
			parameters =>
				parameters.Figure == condition.Owner &&
				RangeHelper.GetFiguresInRange(parameters.AlliedFigure.Hex, 1).Any(figure => figure == condition.PotentialGiver),
			parameters =>
			{
				parameters.SetCannotPass();
			}
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ShackleIndicator shackleIndicator = condition.GetCustomValue<ShackleIndicator>("ShackleIndicator");
		shackleIndicator?.Destroy();

		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(condition);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(condition);
		ScenarioCheckEvents.CanPassAllyCheckEvent.Unsubscribe(condition);
	}
}