// using System.Collections.Generic;
// using System.Linq;
// using Fractural.Tasks;
//
// public class Scenario028 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario028.tscn";
// 	public override int ScenarioNumber => 28;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario029>()];
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals("Destroy the Fountain of Bones to win this scenario.");
//
// 	private Door _door2;
// 	private Objective _fountainOfBones;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ManaMedicine>());
//
// 		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
// 		_fountainOfBones = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
// 		_fountainOfBones.Init(GameController.Instance.SavedCampaign.Characters.Count * (GameController.Instance.SavedScenario.ScenarioLevel + 8),
// 			"Fountain of Bones");
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		if(parameters.Room == GameController.Instance.Map.Rooms[1])
// 		{
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 				canApplyParameters => GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Enemies),
// 				async applyParameters =>
// 				{
// 					await _door2.Unlock();
// 					ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
// 				});
// 			UpdateScenarioText($"Door {Icons.InlineMarker(Marker.Type._2)} is locked and can only be opened when all enemies are dead.");
// 		}
//
// 		if(parameters.Room == GameController.Instance.Map.Rooms[2])
// 		{
// 			UpdateScenarioText(
// 				$"The fountain represents the Fountain of Bones. Whenever a Living Bones is killed, the Fountain of Bones suffers {Icons.Inline(Icons.Damage)}{GameController.Instance.SavedCampaign.Characters.Count + 1}.");
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 				canApplyParameters => canApplyParameters.Figure is Monster monster && monster.MonsterModel is LivingBones,
// 				async applyParameters =>
// 				{
// 					await AbilityCmd.SufferDamage(null, _fountainOfBones, GameController.Instance.SavedCampaign.Characters.Count + 1);
// 				});
// 		}
// 	}
// }

