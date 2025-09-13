using Fractural.Tasks;

public class Immobilize : ConditionModel
{
	public override string Name => "Immobilize";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Immobilize.svg";
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.Performer == Owner && parameters.AbilityState is MoveAbility.State,
			parameters =>
			{
				Node.Flash();
				parameters.SetIsBlocked(true);
				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);

		ScenarioCheckEvents.CanMoveFurtherCheckEvent.Subscribe(this,
			parameters => parameters.Performer == Owner,
			parameters =>
			{
				parameters.SetCannotMoveFurther();
			}
		);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
		ScenarioCheckEvents.CanMoveFurtherCheckEvent.Unsubscribe(this);
	}
}