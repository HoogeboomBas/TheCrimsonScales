using System.Linq;
using Fractural.Tasks;

public class Miser : TheCrimsonScalesBattleGoal
{
	public override string Title => "Miser";
	public override string Description => "Never exit a room with loot tokens in it.";

	public override BattleGoalCheckmarkCount CheckmarkCount => BattleGoalCheckmarkCount.Two;

	public override bool FailIfProgressFull => true;

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		Hex leavingHex = null;

		ScenarioEvents.FigureExitingHexEvent.Subscribe(this,
			parameters => 
				parameters.Figure == character && 
				parameters.Hex.Room != null,
			async parameters =>
			{
				leavingHex = parameters.Hex;

				await GDTask.CompletedTask; 
			}
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters => 
				parameters.Figure == character &&
				parameters.Hex.Room != leavingHex.Room &&
				leavingHex.Room.Hexes.Any(hex => hex.HasHexObjectOfType<Coin>()),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask; 
			}
		);

		await GDTask.CompletedTask;
	}
}