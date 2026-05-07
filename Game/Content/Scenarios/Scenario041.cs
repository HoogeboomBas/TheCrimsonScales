// using System.Collections.Generic;
// using Fractural.Tasks;
//
// public class Scenario041 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario041.tscn";
// 	public override int ScenarioNumber => 41;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<RogueHollowpact>(), "Kill the Rogue Hollowpact to win this scenario.");
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		List<Objective> objectives = GameController.Instance.Map.GetChildrenOfType<Objective>();
// 		int objectiveHealth = GameController.Instance.SavedCampaign.Characters.Count + GameController.Instance.SavedScenario.ScenarioLevel;
// 		foreach(Objective objective in objectives)
// 		{
// 			objective.Init(objectiveHealth, "Void Pit");
// 		}
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this,
// 			canApplyParameters => canApplyParameters.PotentialTarget is Monster monster && monster.MonsterModel is RogueHollowpact &&
// 			                      !GameController.Instance.Map.Rooms[1].Hexes.Contains(canApplyParameters.Performer.Hex),
// 			applyParameters =>
// 			{
// 				applyParameters.SetCannotBeTargeted();
// 			});
//
// 		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
// 			canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is RogueHollowpact &&
// 			                      !GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.Hex.MapTile),
// 			applyParameters =>
// 			{
// 				applyParameters.SetCanEnter(false);
// 			}
// 		);
//
// 		UpdateScenarioText($"""
// 		                    The Dark Pit obstacles represent Void Pit objectives and have C+L hitpoints.
//
// 		                    The Rogue Hollowpact performs the following specials:
// 		                    Special 1: {Icons.Inline(Icons.Move)}+0, {Icons.Inline(Icons.Jump)}, {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Heal)}X, Self, where X is the number of Void Pit obstacles.
// 		                    Special 2: Jump to an empty hex adjacent to a Void Pit obstacle furthest away from a character within {Icons.Inline(Icons.Range)}4. {Icons.Inline(Icons.Attack)}+2, {Icons.Inline(Icons.Range)}4. All enemies adjacent to a Void Pit obstacle suffer {Icons.Inline(Icons.Damage)}2.
//
// 		                    The Rogue Hollowpact will not leave the N1B tile. The Rogue Hollowpact cannot be targeted by any figures that are not occupying the N1b tile.
// 		                    """);
// 	}
// }

