// using System.Collections.Generic;
// using System.Linq;
// using Fractural.Tasks;
// using Godot;
//
// public class Scenario051 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario051.tscn";
// 	public override int ScenarioNumber => 51;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<AshsteelGauntlets>());
// 	}
// }

