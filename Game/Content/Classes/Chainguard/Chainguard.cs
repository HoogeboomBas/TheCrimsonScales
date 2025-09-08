using Fractural.Tasks;

public partial class Chainguard : Character
{
	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, subscriber,
			canApply: parameters => parameters.Condition is Shackle && parameters.PotentialAbilityState != null,
			apply: async parameters =>
			{
				Shackle shackle = (Shackle)parameters.Condition;
				shackle.AddCause(parameters.PotentialAbilityState.Performer);
				await GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals
		);
	}
}