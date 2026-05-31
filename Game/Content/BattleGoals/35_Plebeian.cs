using Fractural.Tasks;

public class Plebeian : TheCrimsonScalesBattleGoal
{
	public override string Title => "Plebeian";
	public override string Description => "Never kill an elite enemy, named enemy, or boss.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialKiller == character &&
				character.EnemiesWith(parameters.Figure) &&
				parameters.Figure is Monster monster &&
				monster.MonsterType is MonsterType.Elite or MonsterType.Named or MonsterType.Boss,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}