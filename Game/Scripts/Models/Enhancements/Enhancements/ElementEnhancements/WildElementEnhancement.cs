using Fractural.Tasks;

public class WildElementEnhancement : EnhancementModel<AbilityState>
{
	public override string TexturePath => Icons.WildElement;
	public override int BaseCost => 150;

	protected override void _Enhance(AbilityState state, EnhancementMark enhancementMark)
	{
		ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
			parameters => parameters.AbilityState == state,
			async parameters =>
			{
				await AbilityCmd.InfuseWildElement(state);
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