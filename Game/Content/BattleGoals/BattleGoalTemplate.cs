using Fractural.Tasks;

public class BattleGoalTemplate : TheCrimsonScalesBattleGoal
{
	public override string Title => "TODO";
	public override string Description => "TODO";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two; //TODO: Remove if only One

	public override int MaxProgress => 1; //TODO: Change if different max progress
	public override bool FailIfProgressFull => true; //TODO: Remove if it's a normal Battle Goal that grants its checkmarks if the progress is full

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		//TODO: Implement
		await GDTask.CompletedTask;
	}
}