using Fractural.Tasks;

public partial class Chainguard : Character
{
	public static Shackle Shackle { get; } = ModelDB.Condition<Shackle>();

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, subscriber,
			canApply: parameters => parameters.Condition is Shackle && 
						parameters.PotentialAbilityState != null &&
						((Shackle)parameters.Condition).Shackler == this,
			apply: async parameters =>
			{
				((Shackle)parameters.Condition).AddShackler(parameters.PotentialAbilityState.Performer);
				await GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals
		);
	}
}