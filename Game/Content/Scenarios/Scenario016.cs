// using Fractural.Tasks;
//
// public class Scenario016 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario016.tscn";
// 	public override int ScenarioNumber => 16;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals("Loot both Goal treasure tiles and kill the Apex Demon to win this scenario.");
//
// 	private Door _door1;
// 	private bool _treasureRoom3Looted;
// 	private bool _treasureRoom4Looted;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		_door1 = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Door>();
//
// 		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(async character =>
// 		{
// 			await AbilityCmd.SufferDamage(character, HazardousTerrain.DamageAmount, character);
// 			await AbilityCmd.AddCondition(null, character, Conditions.Invisible);
// 		});
// 		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async character =>
// 		{
// 			_treasureRoom3Looted = true;
// 			if(_treasureRoom4Looted)
// 			{
// 				await _door1.Unlock();
// 			}
// 		});
// 		GameController.Instance.Map.Treasures[2].SetObtainLootFunction(async character =>
// 		{
// 			_treasureRoom4Looted = true;
// 			if(_treasureRoom3Looted)
// 			{
// 				await _door1.Unlock();
// 			}
// 		});
//
// 		UpdateScenarioText(
// 			$"The door marked {Icons.Inline(Icons.GetMarker(Marker.Type.a))} is locked and becomes unlocked once both Goal treasure tiles have been looted.");
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => KillSpecificEnemiesTypeGoals.SpecificEnemyRemaining([ModelDB.Monster<ApexDemon>()]) && _treasureRoom3Looted &&
// 			              _treasureRoom4Looted,
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			});
// 	}
// }

