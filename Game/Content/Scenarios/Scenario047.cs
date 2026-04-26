// using Fractural.Tasks;
//
// public class Scenario047 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario047.tscn";
// 	public override int ScenarioNumber => 47;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals("");
//
// 	private int _remainingGhostViperKills;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(async lootingCharacter =>
// 		{
// 			await AbilityCmd.GainGold(lootingCharacter, 15);
// 			await GDTask.CompletedTask;
// 		});
//
// 		_remainingGhostViperKills = GameController.Instance.SavedCampaign.Characters.Count + 4;
// 		UpdateScenarioText();
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => _remainingGhostViperKills == 0,
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			}
// 		);
//
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 			parameters => parameters.Figure is Monster monster && monster.MonsterModel is GhostViperScenario047 &&
// 			              _remainingGhostViperKills > 0,
// 			async parameters =>
// 			{
// 				_remainingGhostViperKills--;
// 				UpdateScenarioText();
// 				await GDTask.CompletedTask;
// 			}
// 		);
// 	}
//
// 	private void UpdateScenarioText()
// 	{
// 		GameController.Instance.SpecialRulesView.SetText($"Kill {_remainingGhostViperKills} more Ghost Vipers to win this scenario.");
// 	}
// }

