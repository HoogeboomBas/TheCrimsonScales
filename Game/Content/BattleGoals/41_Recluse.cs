using System.Linq;
using Fractural.Tasks;

public class Recluse : TheCrimsonScalesBattleGoal
{
	public override string Title => "Recluse";
	public override string Description => "Never end your turn adjacent to any other character.";

	public override BattleGoalCheckmarkCount CheckmarkCount => CharacterCount == 4 ? BattleGoalCheckmarkCount.Two : BattleGoalCheckmarkCount.One;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character &&
				parameters.Figure.Hex.Neighbours.Any(hex => hex.GetFigures().Any(figure => figure is Character)),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}