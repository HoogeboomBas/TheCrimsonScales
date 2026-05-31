using Fractural.Tasks;

public class Pauper : TheCrimsonScalesBattleGoal
{
	public override string Title => "Pauper";
	public override string Description => "Never collect a money token from end-of-turn looting.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		bool endOfTurnLooting = false;

		ScenarioEvents.FigureTurnEndingEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character,
			async parameters =>
			{
				endOfTurnLooting = true;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character,
			async parameters =>
			{
				endOfTurnLooting = false;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.CoinLootedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				endOfTurnLooting &&
				parameters.LootObtainer == character,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}