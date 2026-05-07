// using System.Collections.Generic;
// using Fractural.Tasks;
//
// public class Scenario012 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario012.tscn";
// 	public override int ScenarioNumber => 12;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario015>(true)];
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals("Kill all revealed enemies and loot the treasure chest to win this scenario.");
//
// 	private bool _lootedTreasure;
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Available, ItemType.Small,
// 			hintText: $"Select one {Icons.HintText(Icons.GetItem(ItemType.Small))} to {Icons.HintText(Icons.LoseCard)}");
//
// 		if(item != null)
// 		{
// 			await item.SetItemState(ItemState.Consumed);
// 		}
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(OnTreasureLooted);
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => _lootedTreasure && KillAllEnemiesScenarioGoals.NoEnemiesRemaining(),
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			}
// 		);
// 	}
//
// 	private async GDTask OnTreasureLooted(Character lootingCharacter)
// 	{
// 		_lootedTreasure = true;
//
// 		await GDTask.CompletedTask;
// 	}
// }

