using Fractural.Tasks;
using Godot;

public class SpawnEventReward(MonsterModel monsterModel, int maxHP) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor) =>
		$"At the start of the next scenario, an allied {monsterModel.Name} with a maximum hit point value of {maxHP} will spawn next to any character.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		Hex hex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.GetCharacter(0),
			list =>
			{
				foreach(Character character in GameController.Instance.CharacterManager.Characters)
				{
					foreach(Hex hex in RangeHelper.GetHexesInRange(character.Hex, 1, false))
					{
						if(hex.IsEmpty())
						{
							list.AddIfNew(hex);
						}
					}
				}
			}, hintText: $"Select a hex to spawn the allied {monsterModel.Name}"
		);

		if(hex != null)
		{
			Monster monster = await AbilityCmd.SpawnMonster(monsterModel, MonsterType.Normal, hex);
			if(monster != null)
			{
				monster.SetAlignment(Alignment.Characters);
				monster.SetEnemies(Alignment.Enemies);
				monster.SetMaxHealth(maxHP);
				monster.SetHealth(maxHP);
			}
		}
	}
}