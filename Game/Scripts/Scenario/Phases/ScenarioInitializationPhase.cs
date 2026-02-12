using Fractural.Tasks;

public class ScenarioInitializationPhase : ScenarioPhase
{
	public override async GDTask Activate()
	{
		await base.Activate();

		await GameController.Instance.ScenarioModel.StartBeforeFirstRoomRevealed();

		foreach(Room room in GameController.Instance.Map.Rooms)
		{
			if(room.StartsRevealed)
			{
				await room.Reveal(null, true);
			}
		}

		// Set initial positions of all characters
		await GameController.Instance.CharacterManager.PlaceCharacters();

		await GameController.Instance.ScenarioModel.StartAfterFirstRoomRevealed();

		// foreach(SavedEventState savedEventState in GameController.Instance.SavedCampaign.SavedEvents.SavedEventStates)
		// {
		// 	foreach(EventReward eventReward in savedEventState.Choice.GetRewards(savedEventState))
		// 	{
		// 		await eventReward.OnAfterFirstRoomRevealed();
		// 	}
		// }
	}
}