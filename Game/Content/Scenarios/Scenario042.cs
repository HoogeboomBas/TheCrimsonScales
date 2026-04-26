// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario042 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario042.tscn";
// 	public override int ScenarioNumber => 42;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new KillSpecificEnemiesTypeGoals(
// 		[ModelDB.Monster<SlyWolf>(), ModelDB.Monster<GoringGrizzly>(), ModelDB.Monster<KingCobra>()],
// 		"Kill the Goring Grizzly, King Cobra and Sly Wolf to win this scenario.");
//
// 	private Door _door1;
// 	private Door _door2;
// 	private Door _door3;
// 	private IEnumerable<Marker> _markersA;
// 	private IEnumerable<Marker> _markersB;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<WovenPlateArmor>());
// 		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async lootingCharacter =>
// 		{
// 			await AbilityCmd.GainGold(lootingCharacter, 25);
// 			await AbilityCmd.AddCondition(null, lootingCharacter, Conditions.Poison1);
// 		});
// 		GameController.Instance.Map.Treasures[2].SetItemLoot(ModelDB.Item<SteelHelmet>());
//
// 		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
// 		_door1 = marker1.GetHexObject<Door>();
//
// 		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
// 		_door2 = marker2.GetHexObject<Door>();
//
// 		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
// 		_door3 = marker3.GetHexObject<Door>();
//
// 		_markersA = GameController.Instance.Map.GetMarkers(Marker.Type.a);
// 		_markersB = GameController.Instance.Map.GetMarkers(Marker.Type.b);
//
// 		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
// 			parameters => parameters.AbilityState is AttackAbility.State &&
// 			              (parameters.Performer is Character || parameters.Performer is Summon) &&
// 			              GameController.Instance.Map.Rooms[0].MapTiles.Contains(parameters.Performer.Hex.MapTile),
// 			async parameters =>
// 			{
// 				parameters.AbilityState.SetBlocked();
//
// 				await GDTask.CompletedTask;
// 			});
//
// 		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
// 			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1)
// 				.Any(figure => figure.EnemiesWith(parameters.Performer)),
// 			async parameters =>
// 			{
// 				parameters.ForgoAction();
//
// 				ActionState actionState = new ActionState(parameters.Performer, [
// 					PushAbility.Builder()
// 						.WithPush(2)
// 						.WithRange(1)
// 						.Build()
// 				]);
// 				await actionState.Perform();
// 			},
// 			EffectType.Selectable,
// 			effectButtonParameters: new IconEffectButton.Parameters(Icons.Push),
// 			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Push)}2, {Icons.Inline(Icons.Range)}1")
// 		);
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		if(parameters.OpenedDoor == _door1)
// 		{
// 			int houndsToSpawn = 0;
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this, _door1,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel.Name == "Hound" &&
// 				                      GameController.Instance.Map.Figures.Any(figure => figure is Monster monster && monster.MonsterModel is SlyWolf),
// 				async applyParameters =>
// 				{
// 					Figure slyWolf = GameController.Instance.Map.Figures
// 						.First(figure => figure is Monster monster && monster.MonsterModel is SlyWolf);
// 					await AbilityCmd.SufferDamage(slyWolf,
// 						GameController.Instance.SavedCampaign.Characters.Count, slyWolf);
// 					houndsToSpawn++;
// 				});
//
// 			ScenarioEvents.RoundEndedEvent.Subscribe(this, _door1,
// 				canApplyParameters => houndsToSpawn > 0,
// 				async applyParameters =>
// 				{
// 					while(houndsToSpawn > 0)
// 					{
// 						await SpawnMonster(null, ModelDB.Monster<Hound>(), MonsterType.Normal, _markersA.Select(marker => marker.Hex));
// 						houndsToSpawn--;
// 					}
// 				});
//
// 			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door1,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is Hound &&
// 				                      !GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCanEnter(false);
// 				}
// 			);
//
// 			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door1,
// 				canApplyParameters => canApplyParameters.Performer is Monster monster && monster.MonsterModel is Hound &&
// 				                      !GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCannotBeFocused();
// 				});
//
// 			UpdateScenarioText($"""
// 			                    The named Hound is the Sly Wolf and is permanently {Icons.Inline(Icons.GetCondition(Conditions.Invisible))}.
//
// 			                    Whenever a Hound is killed, the Sly Wolf suffers {Icons.Inline(Icons.Damage)}C.
//
// 			                    None of the monsters in the room will focus on enemies outside the D2B tile, nor will they leave the tile. Whenever a Hound is killed, spawn another Hound in one of the hexes marked {Icons.Inline(Icons.GetMarker(Marker.Type.a))} at the end of the round.
// 			                    """);
// 		}
// 		else if(parameters.OpenedDoor == _door2)
// 		{
// 			int caveBearsToSpawn = 0;
// 			int shieldValue = 0;
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this, _door2,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel.Name == "Cave Bear" &&
// 				                      GameController.Instance.Map.Figures.Any(figure =>
// 					                      figure is Monster monster && monster.MonsterModel is GoringGrizzly),
// 				async applyParameters =>
// 				{
// 					Monster goringGrizzly =
// 						(Monster)GameController.Instance.Map.Figures.First(figure =>
// 							figure is Monster monster && monster.MonsterModel is GoringGrizzly);
// 					if(shieldValue > 0)
// 					{
// 						shieldValue--;
// 						ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
// 					}
//
// 					caveBearsToSpawn++;
//
// 					await GDTask.CompletedTask;
// 				});
//
// 			ScenarioCheckEvents.ShieldCheckEvent.Subscribe(this, _door2,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is GoringGrizzly,
// 				applyParameters =>
// 				{
// 					applyParameters.AdjustShield(shieldValue);
// 				});
//
// 			ScenarioEvents.SufferDamageEvent.Subscribe(this, _door2,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is GoringGrizzly &&
// 				                      canApplyParameters.FromAttack,
// 				async applyParameters =>
// 				{
// 					applyParameters.AdjustShield(shieldValue);
// 					await GDTask.CompletedTask;
// 				});
//
// 			ScenarioEvents.RoundEndedEvent.Subscribe(this, _door2,
// 				canApplyParameters => caveBearsToSpawn > 0,
// 				async applyParameters =>
// 				{
// 					while(caveBearsToSpawn > 0)
// 					{
// 						await SpawnMonster(null, ModelDB.Monster<CaveBear>(), MonsterType.Normal, _markersB.Select(marker => marker.Hex));
// 						caveBearsToSpawn--;
// 					}
// 				});
//
// 			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door2,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is CaveBear &&
// 				                      !GameController.Instance.Map.Rooms[2].MapTiles.Contains(canApplyParameters.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCanEnter(false);
// 				}
// 			);
//
// 			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door2,
// 				canApplyParameters => canApplyParameters.Performer is Monster monster && monster.MonsterModel is CaveBear &&
// 				                      !GameController.Instance.Map.Rooms[2].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCannotBeFocused();
// 				});
//
// 			UpdateScenarioText($"""
// 			                    The named Cave Bear is the Goring Grizzly.
//
// 			                    None of the monsters in the room will focus on enemies outside the D1B tile, nor will they leave the tile. Whenever a Cave Bear is killed, reduce the Shield value of the Goring Grizzly by 1 and spawn another Cave Bear in one of the hexes marked {Icons.Inline(Icons.GetMarker(Marker.Type.b))} at the end of the round.
// 			                    """);
// 		}
// 		else if(parameters.OpenedDoor == _door3)
// 		{
// 			ScenarioEvents.AfterSufferDamageEvent.Subscribe(this, _door3,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel.Name == "Giant Viper" &&
// 				                      GameController.Instance.Map.Figures.Any(figure =>
// 					                      figure is Monster monster && monster.MonsterModel is KingCobra),
// 				async applyParameters =>
// 				{
// 					Figure kingCobra =
// 						GameController.Instance.Map.Figures.First(figure => figure is Monster monster && monster.MonsterModel is KingCobra);
// 					await AbilityCmd.SufferDamage(kingCobra, applyParameters.Damage, kingCobra);
//
// 					await GDTask.CompletedTask;
// 				});
//
// 			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door3,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is GiantViper &&
// 				                      !GameController.Instance.Map.Rooms[3].MapTiles.Contains(canApplyParameters.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCanEnter(false);
// 				}
// 			);
//
// 			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door3,
// 				canApplyParameters => canApplyParameters.Performer is Monster monster && monster.MonsterModel is GiantViper &&
// 				                      !GameController.Instance.Map.Rooms[3].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
// 				applyParameters =>
// 				{
// 					applyParameters.SetCannotBeFocused();
// 				});
//
// 			UpdateScenarioText($"""
// 			                    The named Giant Viper is the King Cobra.
//
// 			                    None of the monsters in the room will focus on enemies outside the C2A tile, nor will they leave the tile. Whenever a Giant Viper is damaged, the King Cobra suffers an equal amount of {Icons.Inline(Icons.Damage)}.
// 			                    """);
// 		}
// 	}
// }

