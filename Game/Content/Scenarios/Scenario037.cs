// using System.Collections.Generic;
// using Fractural.Tasks;
//
// public class Scenario037 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario037.tscn";
// 	public override int ScenarioNumber => 37;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario038>(true)];
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new KillAllEnemiesScenarioGoals(customText: "Kill all enemies and destroy all burning stones to win this scenario.");
//
// 	private Objective _radiantStone;
// 	private Objective _flamingStone;
// 	private Objective _shadowStone;
// 	private Objective _frostStone;
// 	private Door _door1;
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<IronSnare>());
//
// 		int burningStoneHealth = GameController.Instance.SavedScenario.ScenarioLevel + GameController.Instance.SavedCampaign.Characters.Count + 2;
//
// 		_radiantStone = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
// 		_radiantStone.Init(burningStoneHealth, "Radiant Stone");
//
// 		_flamingStone = GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>();
// 		_radiantStone.Init(burningStoneHealth, "Flaming Stone");
//
// 		_shadowStone = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();
// 		_radiantStone.Init(burningStoneHealth, "Shadow Stone");
//
// 		_frostStone = GameController.Instance.Map.GetMarker(Marker.Type.d).GetHexObject<Objective>();
// 		_radiantStone.Init(burningStoneHealth, "Frost Stone");
//
// 		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 			parameters => parameters.Figure is Objective,
// 			async parameters =>
// 			{
// 				ScenarioEvents.RoundEndedEvent.Unsubscribe(this, parameters.Figure);
// 				ScenarioCheckEvents.CanConsumeElementCheckEvent.Unsubscribe(this, parameters.Figure);
// 				if(_radiantStone.IsDestroyed && _flamingStone.IsDestroyed)
// 				{
// 					await _door1.Unlock();
// 				}
//
// 				UpdateScenarioText();
// 			});
//
// 		SubscribeStone(_radiantStone, Element.Light);
// 		SubscribeStone(_flamingStone, Element.Fire);
// 	}
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		SubscribeStone(_shadowStone, Element.Dark);
// 		SubscribeStone(_frostStone, Element.Ice);
// 		UpdateScenarioText();
// 	}
//
// 	private void SubscribeStone(Objective burningStone, Element element)
// 	{
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this, burningStone,
// 			parameters => true,
// 			async parameters =>
// 			{
// 				await AbilityCmd.InfuseElement(null, element, immediately: true);
// 			});
//
// 		ScenarioCheckEvents.CanConsumeElementCheckEvent.Subscribe(this, burningStone,
// 			parameters => parameters.Figure is Character && parameters.Element == element,
// 			parameters =>
// 			{
// 				parameters.SetCanConsume(false);
// 			});
// 	}
//
// 	private void UpdateScenarioText()
// 	{
// 		string text = "The burning stones are represented by the boulder obstacles marked with letters.\n\n";
// 		if(!_radiantStone.IsDestroyed)
// 		{
// 			text +=
// 				$"Burning stone {Icons.InlineMarker(Marker.Type.a)} represents the radiant stone. Until the stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Light))} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Light))}\n\n";
// 		}
//
// 		if(!_flamingStone.IsDestroyed)
// 		{
// 			text +=
// 				$"Burning stone {Icons.InlineMarker(Marker.Type.b)} represents the flaming stone. Until the stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Fire))} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Fire))}\n\n";
// 		}
//
// 		if(_door1.Locked)
// 		{
// 			text +=
// 				$"Door {Icons.InlineMarker(Marker.Type._1)} is locked and is unlocked when both burning stone {Icons.InlineMarker(Marker.Type.a)} and {Icons.InlineMarker(Marker.Type.b)} are destroyed.\n\n";
// 		}
//
// 		if(!_shadowStone.IsDestroyed && _door1.Opened)
// 		{
// 			text +=
// 				$"Burning stone {Icons.InlineMarker(Marker.Type.c)} represents the shadow stone. Until the stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Dark))} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Dark))}\n\n";
// 		}
//
// 		if(!_frostStone.IsDestroyed && _door1.Opened)
// 		{
// 			text +=
// 				$"Burning stone {Icons.InlineMarker(Marker.Type.d)} represents the frost stone. Until the stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Ice))} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Ice))}\n\n";
// 		}
//
// 		text = text.TrimEnd('\n');
//
// 		base.UpdateScenarioText(text);
// 	}
// }

