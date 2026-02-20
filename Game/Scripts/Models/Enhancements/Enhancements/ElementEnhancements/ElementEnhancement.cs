using Fractural.Tasks;

public abstract class ElementEnhancement : EnhancementModel<AbilityState>
{
	public override string TexturePath => Icons.GetElement(Element);
	public override int BaseCost => 100;

	protected abstract Element Element { get; }

	protected override void _Enhance(AbilityState state, EnhancementMark enhancementMark)
	{
		ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
			parameters => parameters.AbilityState == state,
			async parameters =>
			{
				await AbilityCmd.InfuseElement(state, Element);
			}
		);

		ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
			parameters => parameters.AbilityState == state,
			async parameters =>
			{
				ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);
				ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);

				await GDTask.CompletedTask;
			}
		);
	}
}