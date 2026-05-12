using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SpawnReward : SavedReward
{
	[JsonProperty]
	private string _monsterModelId;

	[JsonProperty]
	private int _maxHp;

	public MonsterModel MonsterModel => ModelDB.GetById<MonsterModel>(_monsterModelId);

	public override RewardType Type => RewardType.ScenarioStart;

	public SpawnReward()
	{
	}

	public SpawnReward(MonsterModel monsterModel, int maxHp)
	{
		_monsterModelId = monsterModel.Id.ToString();
		_maxHp = maxHp;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"At the start of the next scenario, an allied {MonsterModel.Name} with a maximum hit point value of {_maxHp} will spawn next to any character.";

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
			}, hintText: $"Select a hex to spawn the allied {MonsterModel.Name}"
		);

		if(hex != null)
		{
			Monster monster = await AbilityCmd.SpawnMonster(MonsterModel, MonsterType.Normal, hex, alignment: Alignment.Characters);
			if(monster != null)
			{
				monster.SetMaxHealth(_maxHp);
				monster.SetHealth(_maxHp);
			}
		}
	}
}