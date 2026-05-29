using Fractural.Tasks;

public class Perfectionist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Perfectionist";
	public override string Description => "End the scenario with your hit point value equal to your maximum hit point value.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters => character.Health == character.MaxHealth,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}