// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario036 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario036.tscn";
// 	public override int ScenarioNumber => 36;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new KillSpecificEnemiesTypeGoals([ModelDB.Monster<TerribleTwoBanditArcher>(), ModelDB.Monster<TerribleTwoBanditGuard>()],
// 			"Kill both of the Terrible Two to win this scenario");
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 2).SetItemLoot(ModelDB.Item<HookShot>());
// 		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 30).SetItemLoot(ModelDB.Item<BonecladShawl>());
// 	}
// }

