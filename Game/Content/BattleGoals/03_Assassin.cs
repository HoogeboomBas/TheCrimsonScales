using System.Collections.Generic;
using Fractural.Tasks;

public class Assassin : TheCrimsonScalesBattleGoal
{
	public override string Title => "Assassin";
	public override string Description => "Kill an enemy before it performs any actions.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		List<Figure> actionPerformers = [];

		ScenarioEvents.ActionStartedEvent.Subscribe(this,
			parameters => !battleGoal.ProgressFull,
			async parameters =>
			{
				actionPerformers.AddIfNew(parameters.ActionState.Performer);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialKiller == character &&
				character.EnemiesWith(parameters.Figure) &&
				!actionPerformers.Contains(parameters.Figure),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}