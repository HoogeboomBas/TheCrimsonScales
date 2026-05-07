// using Fractural.Tasks;
//
// public class Scenario054 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario054.tscn";
// 	public override int ScenarioNumber => 54;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		await AbilityCmd.AddConditions(null, character, [Conditions.Curse, Conditions.Curse]);
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<TrophyHelm>());
//
// 		UpdateScenarioText("""
// 		                   The Giant Vipers are Hanging Snakes and have double the number of hit points.
// 		                   The Lurkers are Dark Lurkers and use the Harrower Infester monster ability deck instead of their own.
// 		                   """);
// 	}
//
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
// 	{
// 		await base.OnRoomRevealed(roomRevealedParameters);
// 		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
// 		{
// 			UpdateScenarioText($"""
// 			                    The Living Bones are Undead Archers. Undead Archers gain {Icons.Inline(Icons.Range)}3 and add -1{Icons.Targets} to all their attacks.
// 			                    The Stone Golems are Terror Drones and use the Deep Terror monster ability deck instead of their own. Whenever a Terror Drone would summon a Deep Terror, summon an Undead Archer instead.
// 			                    """);
// 			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
// 				parameters => parameters.Performer is Monster monster && monster.MonsterModel is TerrorDrone &&
// 				              parameters.AbilityState is MonsterSummonAbility.State summonState && summonState.MonsterModel is DeepTerror,
// 				async parameters =>
// 				{
// 					((MonsterSummonAbility.State)parameters.AbilityState).SetMonsterModel(ModelDB.Monster<UndeadArcher>());
// 					await GDTask.CompletedTask;
// 				});
// 		}
// 		else if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
// 		{
// 			UpdateScenarioText("""
// 			                   The Cave Bear is the Black Bear and uses the Night Demon monster ability deck instead of their own. All attacks targeting the Black Bear gain Disadvantage.
// 			                   The Forest Imps are Cave Imps and have double the number of hit points. Cave Imps use the Ancient Artillery monster ability deck instead of their own.
// 			                   """);
// 		}
// 	}
// }

