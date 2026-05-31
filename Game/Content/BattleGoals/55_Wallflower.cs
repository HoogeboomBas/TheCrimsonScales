using System.Linq;
using Fractural.Tasks;

public class Wallflower : TheCrimsonScalesBattleGoal
{
	public override string Title => "Wallflower";
	public override string Description => "End each of your turns adjacent to a wall, obstacle or objective.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character &&
				character.Hex.Neighbours.Count == 6 && // This should check for wall lines
				RangeHelper.GetHexesInRange(character.Hex, 1, false)
					.All(hex => !hex.HasHexObjectOfType<Obstacle>() || !hex.HasHexObjectOfType<Objective>()),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}