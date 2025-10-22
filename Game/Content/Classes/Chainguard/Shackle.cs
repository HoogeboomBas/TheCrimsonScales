using System.Linq;
using Fractural.Tasks;
using Godot;

public class Shackle : ConditionModel
{
	public override string Name => "Shackle";
	public override string IconPath => "res://Content/Classes/Chainguard/Shackle.svg";
	public override bool RemovedByHeal => false;
	public override bool CanBeUpgraded => false;
	public override bool IsPositive => false;
	public override bool IsNegative => false;
	public override bool ShowOnFigure => false;
	public override ConditionModel ImmunityCompareBaseCondition => Conditions.Immobilize;

	public Figure Shackler { get; private set; }

	private ShackleIndicator _indicator;

	public void SetShackler(Figure shackler)
	{
		Shackler = shackler;
	}

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		_indicator = ResourceLoader.Load<PackedScene>("res://Content/Classes/Chainguard/ShackleIndicator.tscn").Instantiate<ShackleIndicator>();
		target.AddChild(_indicator);
		_indicator.Init();

		// Stop movement if became adjacent to the Shackler
		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(target, this,
			parameters => parameters.Performer == Owner &&
			              RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1).Any(figure => figure == Shackler),
			async parameters =>
			{
				_indicator.Flash();
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
				_indicator.Flash();
				parameters.SetIsBlocked(true);

				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		_indicator?.Destroy();

		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(Owner, this);
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(Owner, this);
	}
}