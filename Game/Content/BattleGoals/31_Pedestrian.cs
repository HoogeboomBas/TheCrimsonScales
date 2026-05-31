using System.Linq;
using Fractural.Tasks;

public class Pedestrian : TheCrimsonScalesBattleGoal
{
	public override string Title => "Pedestrian";
	public override string Description => "Never enter a hex occupied by an ally, enemy, objective or obstacle.";

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character &&
					(parameters.Hex.GetFigures().Any(figure => figure.AlliedWith(character) || figure.EnemiesWith(character)) ||
					parameters.Hex.HasHexObjectOfType<Objective>() ||
					parameters.Hex.HasHexObjectOfType<Obstacle>()),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}