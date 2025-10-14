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

		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(-1, OnTreasureLooted, null);

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

	private async GDTask OnTreasureLooted(Character lootingCharacter)
	{
		_lootedTreasure = true;

		await GDTask.CompletedTask;
	}
}