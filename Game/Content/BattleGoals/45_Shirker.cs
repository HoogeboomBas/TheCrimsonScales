using System.Linq;
using Fractural.Tasks;

public class Shirker : TheCrimsonScalesBattleGoal
{
	public override string Title => "Shirker";
	public override string Description => "Kill an enemy not adjacent to you while you are adjacent to another enemy.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.PotentialKiller == character &&
				character.EnemiesWith(parameters.Figure) &&
				RangeHelper.Distance(character.Hex, parameters.Figure.Hex) > 1 &&
				RangeHelper.GetFiguresInRange(character, 1, false, false).Any(figure => character.EnemiesWith(figure)),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}