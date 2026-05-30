using Fractural.Tasks;

public class Workhorse : TheCrimsonScalesBattleGoal
{
	public override string Title => "Workhorse";
	public override string Description => "Gain 13 or more experience before any bonus scenario experience.";

	public override int MaxProgress => 13;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.GainedExperienceEvent.Subscribe(this,
			parameters =>
				parameters.ExperienceReceiver == character &&
				!parameters.FromScenario,
			async parameters =>
			{
				battleGoal.AdjustProgress(parameters.ExperienceAmount);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}