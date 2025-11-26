using Fractural.Tasks;

public class PushTrait(int amount) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.DuringAttackEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustPush(amount);

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.DuringAttackEvent.Unsubscribe(figure, this);
	}
}