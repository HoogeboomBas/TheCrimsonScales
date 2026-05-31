using Fractural.Tasks;

public class Weakling : TheCrimsonScalesBattleGoal
{
	public override string Title => "Weakling";
	public override string Description => "Become exhausted before any other character.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				!battleGoal.ProgressFull &&
				parameters.Figure == character,
			async parameters =>
			{
				bool success = true;
				foreach(Character otherCharacter in GameController.Instance.CharacterManager.Characters)
				{
					if(otherCharacter != character && otherCharacter.IsDead)
					{
						success = false;
					}
				}

				if(success)
				{
					battleGoal.AdjustProgress(1);
				}

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}