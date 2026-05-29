using System.Linq;
using Fractural.Tasks;

public class Ritualist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Ritualist";
	public override string Description => "Kill an enemy while three or more elements are strong or waning.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.PotentialKiller == character &&
				parameters.Figure.EnemiesWith(character) &&
				Elements.All.Count(element => GameController.Instance.ElementManager.GetState(element) is ElementState.Waning or ElementState.Strong) > 3,
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}