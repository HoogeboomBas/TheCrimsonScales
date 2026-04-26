// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario040 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario040.tscn";
// 	public override int ScenarioNumber => 40;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new KillSpecificEnemiesTypeGoals(ModelDB.Monster<DrakePorter>(), "Kill the Drake Porter to win this scenario.");
//
// 	private Door _door1;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
// 		_door1 = marker1.GetHexObject<Door>();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<SteelHelmet>());
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this, _door1,
// 			canApply: parameters =>
// 			{
// 				Figure character = GameController.Instance.Map.Figures.FirstOrDefault(figure => figure is Character);
// 				return GameController.Instance.Map.Figures.Any(figure => figure.EnemiesWith(character));
// 			},
// 			apply: async parameters =>
// 			{
// 				ScenarioEvents.FigureKilledEvent.Unsubscribe(this, _door1);
// 				UpdateScenarioText(null);
// 				await _door1.Unlock();
// 			}
// 		);
//
// 		UpdateScenarioText($"The door is locked until all revealed enemies are killed.");
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		Figure drakePorter =
// 			GameController.Instance.Map.Figures.First(figure => figure is Monster monster && monster.MonsterModel is DrakePorter);
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this, _door1,
// 			canApply: parameters =>
// 				parameters.Figure is Monster monster && (monster.MonsterModel is RendingDrake || monster.MonsterModel is SpittingDrake),
// 			apply: async parameters =>
// 			{
// 				if(!drakePorter.IsDead)
// 				{
// 					await AbilityCmd.SufferDamage(parameters.PotentialAbilityState, drakePorter, 2);
// 					//TODO: add state
// 				}
//
// 				await GDTask.CompletedTask;
// 			}
// 		);
// 		UpdateScenarioText($"""
// 		                    The Drake Porter draws from the boss ability deck.
//
// 		                    Every time you kill a drake, the Drake Porter suffers 2 damage.
// 		                    """);
// 		await GDTask.CompletedTask;
// 	}
// }

