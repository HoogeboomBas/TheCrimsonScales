using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ScenarioInitializationPhase : ScenarioPhase
{
	public override async GDTask Activate()
	{
		await base.Activate();

		await GameController.Instance.ScenarioModel.InitializeBeforeFirstRoomRevealed();

		foreach(Room room in GameController.Instance.Map.Rooms)
		{
			if(room.StartsRevealed)
			{
				await room.Reveal(null, null, true);
			}
		}

		// Set initial positions of all characters
		await GameController.Instance.CharacterManager.PlaceCharacters();

		// Give all characters battle goals to pick from
		List<BattleGoalModel> battleGoals = BattleGoals.Goals.ToList();
		battleGoals.Shuffle(GameController.Instance.StateRNG);
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			for(int i = 0; i < 3; i++)
			{
				character.AddAvailableBattleGoal(battleGoals[0]);
				battleGoals.RemoveAt(0);
			}
		}

		await GameController.Instance.ScenarioModel.InitializeAfterFirstRoomRevealed();

		await GameController.Instance.OpenStoryViewIntroduction();
	}
}