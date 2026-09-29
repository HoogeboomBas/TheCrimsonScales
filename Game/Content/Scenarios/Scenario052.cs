using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario052 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario052.tscn";

	public override int ScenarioNumber => 52;
	public override string Name => "Wishing Well";

	public override List<ScenarioLink> Links => [GloomhavenLink.Instance];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string IntroductionText =>
		"""
		There is an old wishing well located somewhere between the Horn District and the Ward of Scales. It is frequently visited by the poor Inox workers or young couples who toss in the few coins they can spare to wish for a better life, healthy babies or everlasting love and happiness. Lately, strange noises are coming from the well and people are avoiding the place, especially at night, when a foul stench rises from the deep. The city workers refuse to go down the sewers, so the administrator has hired you to investigate the well and clean up whatever mess you might find down there.

		It is early in the afternoon when enter the square with the wishing well in the middle. There is nothing special to see from up here so you toss in a coin and make a wish to see what happens. You hear no splash. Instead, strange sounds are emerging from the hole. They sounds like words! You toss in another coin a listen closely. You think you recognize the word “more”, so you toss in another coin, but the same word is repeated over and over. Your purse is almost empty and you’ve had it with this greedy well. You’re going down to get your money back!

		You enter the underground Gloomhaven water supply system and take some water-breathing orbs with you, just in case. When you walk up to the hole where the well should be, you see it has dried up. There’s no more water flowing in from the pipes to the left and right. There are, however, some green slimes collecting all the money people throw in.

		You don’t know what happened here. Perhaps someone fool-heartedly made a wish? At the other side of the well, unreachable for now, you see an overflow shaft with two connected levers. If you could reach these, the well would fill up in an instant. However, the distant sound of clapping wings and some red scales at the bottom of the shaft tell you it is inhabited by drakes. Time to get to work and clean up this mess!
		""";

	public override string ConclusionText =>
		"""
		When you finally pull the levers of the overflow valve, the well quickly fills up all the way to the top. You thank the Great Oak that you took the water-breathing orbs with you as you watch the remaining creatures around you slowly drown. The few people on the square are startled as you crawl out of the well, all soaked and dripping.

		You still wonder what happened down there. Did the poor city workers accidentally lock themselves in? Or was it perhaps their punishment for trying to steal the coins from the well? You will probably never know, but at least the wish of the city administrator has been granted. The well is all cleaned up.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<Ooze>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<DrakescaleShield>())
	];

	private List<PressurePlate> _pressurePlatesA;
	private List<PressurePlate> _pressurePlatesB;
	private Hex _markerCHex;

	private CustomScenarioGoal _goal;

	private ScenarioRule _pressurePlateARule1;
	private ScenarioRule _pressurePlateARule2;
	private ScenarioRule _darkPitRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(textParameters =>
				$"Both pressure plates {Icons.InlineMarker(Marker.Type.b, textParameters)} are occupied at the end of any round.",
			hasProgress: true, maxProgress: 2));

		_darkPitRule = AddScenarioRule(textParameters =>
			$"Dark Pit overlay tiles cannot be moved or destroyed in any way and cannot be entered or moved through by characters or character summons. Character and character summons cannot occupy the E1A tile until both pressure plates {Icons.InlineMarker(Marker.Type.a, textParameters)} have been removed.");
		AddScenarioRule(textParameters =>
			$"Oozes drop two money tokens instead of one.");
		AddScenarioRule(textParameters =>
			$"At the end of every even round, spawn a Spitting Drake at {Icons.InlineMarker(Marker.Type.c, textParameters)}. {(GameController.Instance.SavedCampaign.Characters.Count == 2 ? "All spawns are normal" : GameController.Instance.SavedCampaign.Characters.Count == 3 ? "Every other spawn is elite" : "All spawns are elite")}.");

		_pressurePlatesA = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<PressurePlate>()).ToList();
		_pressurePlatesB = GameController.Instance.Map.GetMarkers(Marker.Type.b).Select(marker => marker.GetHexObject<PressurePlate>()).ToList();
		_markerCHex = GameController.Instance.Map.GetMarker(Marker.Type.c).Hex;

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character or Summon &&
				(parameters.Hex.MapTile == GameController.Instance.Map.Rooms[0].MapTiles[1] ||
				 parameters.Hex.HasHexObjectOfType<DarkPitObstacle>()),
			parameters =>
			{
				parameters.SetCanEnter(false);
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber % 2 == 0,
			async parameters =>
			{
				await SpawnMonster(null, ModelDB.Monster<SpittingDrake>(), CalculateMonsterType(parameters.RoundNumber), _markerCHex);
			}
		);

		ScenarioCheckEvents.SpawnCoinCheckEvent.Subscribe(this,
			parameters => parameters.Dropper is Monster monster && monster.MonsterModel is Ooze,
			parameters =>
			{
				parameters.SetCoinsToSpawn(2);
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _pressurePlatesA,
			parameters => _pressurePlatesA.Select(pressurePlate => pressurePlate.Hex).Contains(parameters.Figure.Hex),
			async parameters =>
			{
				PressurePlate pressurePlate = parameters.Figure.Hex.GetHexObjectOfType<PressurePlate>();
				await pressurePlate.Destroy();
				_pressurePlatesA.Remove(pressurePlate);
				if(_pressurePlatesA.Count == 0)
				{
					await PressurePlatesADestroyed();
				}
			}
		);
	}


	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(!GameController.Instance.Map.Rooms.All(room => room.Revealed))
		{
			_pressurePlateARule1 = AddScenarioRule(textParameters =>
				$"""
				 Whenever a pressure plate marked {Icons.InlineMarker(Marker.Type.a, textParameters)} is occupied at the end of a turn, remove it from the board.
				 """
			);

			_pressurePlateARule2 = AddScenarioRule(textParameters =>
				$"""
				 Something will happen when both pressure plates {Icons.InlineMarker(Marker.Type.a, textParameters)} have been removed.
				 """
			);
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			await ShowText(
				"""
				It takes some effort to break open the door and when it finally swings wide, you are greeted with vile imps and dehydrated living corpses wearing city worker uniforms. In the back of the room you notice a rusty lever. No doubt it will unblock one of the pipes to the well.
				""");
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			await ShowText(
				"""
				You have to put all your weight against the door to make it budge. A living corpse holding a wrench stumbles towards you, flanked by some imps. There is a lever in the back that might open one of the pipes to the well.
				""");
		}
	}

	private MonsterType CalculateMonsterType(int roundNumber)
	{
		if(GameController.Instance.SavedCampaign.Characters.Count > 3 ||
		   (roundNumber % 4 == 0 && GameController.Instance.SavedCampaign.Characters.Count == 3))
		{
			return MonsterType.Elite;
		}

		return MonsterType.Normal;
	}

	private async GDTask PressurePlatesADestroyed()
	{
		ScenarioCheckEvents.CanEnterCheckEvent.Unsubscribe(this);
		List<Hex> hexes = [];
		foreach(DarkPitObstacle darkPit in GameController.Instance.Map.GetChildrenOfType<DarkPitObstacle>())
		{
			hexes.AddRange(darkPit.Hexes);
			await darkPit.Destroy(forceDestroy: true);
		}

		foreach(Hex hex in hexes)
		{
			await AbilityCmd.CreateDifficultTerrain(hex,
				SceneLoader.LoadPackedScene("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn"));
		}

		ScenarioEvents.SufferDamageEvent.Subscribe(this,
			parameters => parameters.Figure is Character character && character.ObtainedCoins > 0 && parameters.WouldSufferDamage &&
			              RangeHelper.GetHexesInRange(character.Hex, 1).Any(hex => hex.HasHexObjectOfType<Water>()),
			async parameters =>
			{
				((Character)parameters.Figure).RemoveCoin();
				parameters.SetDamagePrevented();
				await GDTask.CompletedTask;
			}, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/Other/Coin.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Return one money token to the supply to negate the {Icons.Inline(Icons.Damage)}")
		);

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await _goal.SetProgress(_pressurePlatesB.Count(pressurePlate => pressurePlate.Hex.IsOccupied()));
			}
		);

		_darkPitRule.Remove();
		_pressurePlateARule1.Remove();
		_pressurePlateARule2.Remove();

		AddScenarioRule(textParameters =>
			$"Characters adjacent to or occupying a water hex that would suffer {Icons.Inline(Icons.Damage, textParameters)} from an attack may return one money token from their possession to the supply (tossing it in the wishing well) to negate the {Icons.Inline(Icons.Damage, textParameters)}.");
	}
}