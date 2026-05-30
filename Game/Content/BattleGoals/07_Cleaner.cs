using Fractural.Tasks;

public class Cleaner : TheCrimsonScalesBattleGoal
{
	public override string Title => "Cleaner";
	public override string Description => "Collect three or more loot tokens in the same turn.";

	public override int MaxProgress => 3;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.CoinLootedEvent.Subscribe(character, this,
			parameters => 
				parameters.LootObtainer == character &&
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(character, this,
			parameters => 
				parameters.Figure == character &&
				!battleGoal.ProgressFull,
			async parameters =>
			{
				battleGoal.ResetProgress();

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}