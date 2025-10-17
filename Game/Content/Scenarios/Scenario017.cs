using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario017 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario017.tscn";
	public override int ScenarioNumber => 17;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario002>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();

	public override string BGSPath => null;

	private bool _lootedTreasure;

	public override async GDTask StartBeforeFirstRoomRevealed()
	{
		await base.StartBeforeFirstRoomRevealed();

		UpdateScenarioText(
			$"At the start of the scenario, nominate one character to carry the Frosted Crystal. " +
			$"This character may not loot the goal tile and gains {Icons.Inline(Icons.Retaliate)} 1.");

		Treasure firstTreasure = GameController.Instance.Map.Treasures[0];

		firstTreasure.SetObtainLootFunction(6,
			async character =>
			{
				//TODO: Show popup with rewards
				foreach(Hex neighbourHex in firstTreasure.Hex.Neighbours)
				{
					foreach(Trap trap in neighbourHex.GetHexObjectsOfType<Trap>())
					{
						await trap.Disarm();
					}
				}
			},
			character =>
			{
				character.SavedCharacter.AddGold(20);
			}
		);

		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(19,
			async character =>
			{
				//TODO: Show popup with rewards
				await AbilityCmd.InfuseWildElement(character);
			},
			character =>
			{
				//TODO: Add checkmark
				character.SavedCharacter.AddGold(20);
			}
		);

		GameController.Instance.Map.Treasures[2].SetObtainLootFunction(43,
			async character =>
			{
				//TODO: Show popup with rewards
				await AbilityCmd.GainXP(character, 10);

				foreach(ItemModel item in character.Items)
				{
					if(item.ItemState == ItemState.Spent)
					{
						await AbilityCmd.RefreshItem(item);
					}
				}
			},
			null
		);

		GameController.Instance.Map.Treasures[3].SetObtainLootFunction(-1, OnGoalTreasureLooted, null);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters =>
			{
				if(!_lootedTreasure)
				{
					return false;
				}

				foreach(Character character in GameController.Instance.CharacterManager.Characters)
				{
					if(!character.IsDead && !character.Hex.HasHexObjectOfType<StartHexIndicator>())
					{
						return false;
					}
				}

				return true;
			},
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);

		ScenarioEvents.ScenarioSetupCompletedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await GameController.Instance.CharacterManager.AddStartHexIndicators();
			}
		);
	}

	private async GDTask OnGoalTreasureLooted(Character lootingCharacter)
	{
		_lootedTreasure = true;

		await GDTask.CompletedTask;
	}
}