using Fractural.Tasks;

public class Stun : ConditionModel
{
	public override string Name => "Stun";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Stun.svg";
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.Performer == Owner && !parameters.AbilityState.PerformWhileStunned,
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

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
	}
}