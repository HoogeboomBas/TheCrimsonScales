using Fractural.Tasks;

public class Sluggard : TheCrimsonScalesBattleGoal
{
	public override string Title => "Sluggard";
	public override string Description => "Perform a long rest while at your maximum hit point value, after you have already suffered damage.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		bool sufferedDamage = false;

		ScenarioEvents.AfterSufferDamageEvent.Subscribe(this,
			parameters => parameters.Figure == character,
			async parameters =>
			{
				sufferedDamage = true;

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.LongRestStartedEvent.Subscribe(this,
			parameters => 
				parameters.Character == character && 
				character.Health == character.MaxHealth &&
				sufferedDamage == true,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}