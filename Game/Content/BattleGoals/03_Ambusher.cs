using System.Linq;
using Fractural.Tasks;

public class Ambusher : TheCrimsonScalesBattleGoal
{
	public override string Title => "Ambusher";
	public override string Description => "Open a door and end your move ability adjacent to an enemy in the revealed room in the same turn.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		Room revealedRoom = null;

		ScenarioEvents.RoomRevealedEvent.Subscribe(this,
			parameters =>
				parameters.OpenedDoor != null &&
				parameters.PotentialOpener == character,
			async parameters =>
			{
				revealedRoom = parameters.Room;

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.AbilityEndedEvent.Subscribe(this,
			parameters =>
				revealedRoom != null &&
				parameters.AbilityState is MoveAbility.State moveAbilityState &&
				moveAbilityState.Performer == character &&
				moveAbilityState.Hexes.Count > 0 && RangeHelper.GetFiguresInRange(moveAbilityState.Hexes[^1], 1, false, false)
					.Any(figure => character.EnemiesWith(figure) && figure.Hex.Room == revealedRoom),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				revealedRoom = null;

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}