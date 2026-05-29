using System.Linq;
using Fractural.Tasks;

public class Egoist : TheCrimsonScalesBattleGoal
{
	public override string Title => "Egoist";
	public override string Description => "Collect more loot tokens than any other character.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.ScenarioEndedEvent.Subscribe(this,
			parameters =>
			{
				bool success = true;
				foreach(Character otherCharacter in GameController.Instance.CharacterManager.Characters.Except([character]))
				{
					if(otherCharacter.ObtainedCoins >= character.ObtainedCoins)
					{
						success = false;
						break;
					}
				}

				return success;
			},
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			});

		await GDTask.CompletedTask;
	}
}