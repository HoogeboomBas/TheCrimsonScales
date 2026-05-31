using Fractural.Tasks;

public class Layabout : TheCrimsonScalesBattleGoal
{
	public override string Title => "Layabout";
	public override string Description => "Gain 7 or fewer experience before any bonus scenario experience.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override int MaxProgress => 8;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.GainedExperienceEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
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