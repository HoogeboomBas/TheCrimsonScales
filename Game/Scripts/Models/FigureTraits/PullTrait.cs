using Fractural.Tasks;

public class PullTrait(int amount) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.DuringAttackEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustPull(amount);

				await GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.DuringAttackEvent.Unsubscribe(figure, this);
	}
}