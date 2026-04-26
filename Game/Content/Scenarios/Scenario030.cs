// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario030 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario030.tscn";
// 	public override int ScenarioNumber => 30;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
// 	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario032>()];
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new KillSpecificEnemiesTypeGoals([ModelDB.Monster<ShadowDemon>(), ModelDB.Monster<TwinCorpse>()],
// 			"Kill all the Shadow Demon and Twin Corpses to win this scenario");
//
// 	private Door _door2;
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
//
// 		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 17).SetItemLoot(ModelDB.Item<ConcussionMine>());
// 		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 38).SetItemLoot(ModelDB.Item<WarPick>());
//
// 		UpdateScenarioText($"Door {Icons.InlineMarker(Marker.Type._2)} is locked until door {Icons.InlineMarker(Marker.Type._1)} has been opened.");
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		if(parameters.Room == GameController.Instance.Map.Rooms[1])
// 		{
// 			await _door2.Unlock();
//
// 			UpdateScenarioText(
// 				$"The elite Night Demon is the Shadow Demon.");
// 		}
//
// 		if(parameters.Room == GameController.Instance.Map.Rooms[2])
// 		{
// 			int summonCount = GameController.Instance.SavedCampaign.Characters.Count + 2;
// 			
// 			UpdateScenarioText($"""
// 								The Living Corpses are the Twin Corpses.
// 								Whenever a Twin Corpse is killed, summon {summonCount} normal Living Corpses in unoccupied hexes nearest to the hex in which it was killed.
// 								""");
//
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 				figureKilledParameters => figureKilledParameters.Figure is Monster monster && monster.MonsterModel is TwinCorpse,
// 				async figureKilledParameters =>
// 				{
// 					for(int monsterIndex = 0; monsterIndex < summonCount; monsterIndex++)
// 					{
// 						await SummonMonster(null, ModelDB.Monster<LivingCorpse>(), MonsterType.Normal, figureKilledParameters.Figure.Hex);
// 					}
// 				});
// 		}
// 	}
// }

