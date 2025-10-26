using Fractural.Tasks;

public class JumpTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && parameters.AbilityState is MoveAbility.State,
			async parameters =>
			{
				MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
				moveAbilityState.AddJump();

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
	}
}