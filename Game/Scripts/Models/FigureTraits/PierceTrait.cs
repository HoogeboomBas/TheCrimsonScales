using Fractural.Tasks;

public class PierceTrait(int pierce) : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustPierce(pierce);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.PierceCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.AdjustPierce(pierce);
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.PierceCheckEvent.Unsubscribe(figure, this);
	}
}