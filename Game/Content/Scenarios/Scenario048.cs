using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Scenario048 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario048.tscn";

	public override int ScenarioNumber => 48;
	public override string Name => "Forest Fire";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		After agreeing to help extinguish spot fires during the ongoing wildfire, the Fire Knight provides your party with extra Fire Brigade uniforms to protect your skin from the flames before sending you on your way to patrol a nearby section of the forest. In your conversation with the Fire Knight, you learned that one of the lnox tribes of the Corpsewood Forest started this blaze. You suspect that it might be an effort to distract the City Guard long enough to launch an assault against the city. They have never been happy about the city-dwellers building homes and cottages deeper and deeper into forested lands.

		Despite the specialized gear you have been provided along with a quick lesson from the Fire Knight about how to properly smother the flames with nothing but the surrounding dirt, you are unsettled by the daunting task ahead of you. As if firefighting weren’t dangerous enough, you will also have to defend yourself from this rogue lnox tribe who will certainly not be happy to see you trying to undo their efforts. You can only hope the paycheck will be worth it in the end.

		You catch a whiff of smoke as you approach a clearing. Sure enough, where there’s smoke, there’s fire. Lurking behind the trees, you spot a couple lnox along with their wild pets, both of which are vicious enough to send you to the medical ward. Time to earn your keep and show he Fire Knight what you can do.
		""";

	public override string ConclusionText =>
		"""
		With the last of the spot fires extinguished, you turn, braced for another wave of battle, but are surprised to find the lnox retreating instead. With their plans thwarted and their skill in combat outmatched, they apparently have no interest in continuing to fight. Before long, the Fire Knight returns from the front lines to speak with you. “Ah, there you are. Glad to see the flames didn’t eat you up,” he says with a smirk. “We’ve got the fire all but contained at the front. How did you fare back here?” After recounting your experience, the Fire Knight responds, “Hmm, Chief will be interested to hear about this lnox tribe that started the fire, since more will be sure to follow.”

		“Speaking of Chief.” the Fire Knight exclaims after a brief pause, “he sent your payday along and made me swear to thank you properly. We appreciate your help! And hey, with this experience, maybe you’ll even join the Brigade when you retire too!” You share a laugh with your former companion and trade stories all the way back to Gloomhaven. Time to enjoy your payday with your old pal at the Sleeping Lion.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<InoxShaman>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(10),
		new GainGoldEachReward(20)
	];

	private CustomScenarioGoal _goal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(
			textParameters => "Extinguish all fires.", hasProgress: true,
			maxProgress: GameController.Instance.CharacterManager.Characters.Count == 2 ? 14 : 17));

		AddScenarioRule(
			"""
			The hot coal overlay tiles in this scenario represent fire, and are considered difficult terrain instead of hazardous terrain for all characters.
			""");

		AddScenarioRule(
			"""
			Whenever a character ends their turn in a hex that is on fire, that single hex of fire is extinguished and can be removed from the board.
			""");

		if(GameController.Instance.SavedCampaign.Characters.Count >= 3)
		{
			foreach(Marker marker in GameController.Instance.Map.GetMarkers(Marker.Type.a))
			{
				marker.GetHexObject<DifficultTerrain>()?.Destroy(true, true);
			}
		}

		if(GameController.Instance.CharacterManager.Characters.Count == 2)
		{
			Door door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
			await door2.Destroy(true, true);

			Vector2I coords = new Vector2I(5, -7);
			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
				parameters =>
					parameters.Hex.Coords == coords,
				parameters =>
					parameters.SetCanEnter(false), order: 100000000
			);

			foreach(MapTile mapTile in GameController.Instance.Map.Rooms[2].MapTiles)
			{
				mapTile.Sprite.SetVisible(false);
			}
		}

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character &&
				(parameters.Figure.Hex.HasHexObjectOfType<HotCoals>()),
			async parameters =>
			{
				await parameters.Figure.Hex.GetHexObjectOfType<HotCoals>().Destroy();
				await _goal.AdjustProgress(1);
			}, //EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Hot Coals 1h.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Extinguish fire (Remove hot coals from the board).")
		);

		ScenarioCheckEvents.MoveCheckEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.Performer is Character &&
				canApplyParameters.Hex.HasHexObjectOfType<HotCoals>(),
			applyParameters =>
			{
				applyParameters.SetMoveCost(2);
				applyParameters.SetAffectedByNegativeHex(false);
			}
		);

		ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.PotentialAbilityState?.Performer is Character &&
				canApplyParameters.Hex.HasHexObjectOfType<HotCoals>(),
			applyParameters =>
			{
				applyParameters.SetAffectedByHazardousTerrain(false);
				return GDTask.CompletedTask;
			}
		);
	}
}