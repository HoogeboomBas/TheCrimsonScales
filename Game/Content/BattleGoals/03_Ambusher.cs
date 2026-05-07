using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Ambusher : TheCrimsonScalesBattleGoal
{
	public override string Title => "Ambusher";
	public override string Description => "Open a door and end your move ability adjacent to an enemy in the revealed room in the same turn.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		List<Room> revealedRooms = new List<Room>();

		ScenarioEvents.RoomRevealedEvent.Subscribe(this,
			parameters =>
				parameters.OpenedDoor != null &&
				parameters.PotentialOpener == character,
			async parameters =>
			{
				revealedRooms.Add(parameters.Room);

				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.AbilityEndedEvent.Subscribe(this,
			parameters =>
				revealedRooms.Count > 0 &&
				parameters.AbilityState is MoveAbility.State moveAbilityState &&
				moveAbilityState.Performer == character &&
				moveAbilityState.Hexes.Count > 0 && RangeHelper.GetFiguresInRange(moveAbilityState.Hexes[^1], 1, false, false)
					.Any(figure => character.EnemiesWith(figure) && revealedRooms.Contains(figure.Hex.Room)),
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
				revealedRooms.Clear();

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}