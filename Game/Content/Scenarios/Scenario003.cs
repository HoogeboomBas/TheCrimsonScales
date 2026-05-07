using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class Scenario003 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario003.tscn";

	public override int ScenarioNumber => 3;
	public override string Name => "Flooded Cavern";

	public override List<ScenarioLink> Links => [new ScenarioLink<Scenario002>()];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario004>(), new ScenarioConnection<Scenario007>()];

	public override string IntroductionText =>
		"""
		The tidal wave of filthy water carries you into a large cavern, the biggest you’ve seen yet. It bears a resemblance to the previous channels, with its brick interior overwhelmed by natural growths, but there seems to be no Quatryl wizardry here.

		As quickly as the wave picked you up, it recedes again, depositing you on the far side of the cavern. However, you can see the next wave approaching and the tide is definitely coming in.

		You also notice at the head of the water, like a figurehead on a ship, a giant version of the water spirits and, at one side of the cavern, a glowing white crystal. It is within reach, if you can defeat the Hydra Spirit first.
		""";

	public override string ConclusionText =>
		"""
		With a last defiant cry, the Hydra Spirit falls backwards and returns to the water it came from. The water recedes, and allows you to grab the crystal. It is freezing to the touch and you have to wrap it in several layers of cloth before you can secure it in your pack, though you can still feel an eerie chill as you return to the open air.

		Returning to Gloomhaven, you head back to the scruffy tavern where you first spoke to the hooded figure. Not really knowing what to do, you pick the obvious option and order a round of drinks. A while later, you realize the hooded figure is studying you from the bar. You have no idea how long he has been there, which you find slightly unsettling, but as you make eye contact, he walks softly over to your table.

		Half-smiling, he simply says “I wasn’t sure I’d see you again.” Slightly riled by this slight on your abilities, you don’t reply, but merely pull the wrapped crystal from your pack and slide it over the table. The man slightly recoils at this, and his eyes widen. “Not here,” he says and tips his head towards the door. “Follow me.”

		The shabby cloak camouflages the man perfectly, and his cat-like movement makes no sound on the cobble, making him hard to follow as he strides quickly down a dark passage opposite the inn.

		Looking furtively around, he suddenly leans on a door that blends into the wall almost perfectly, whether by accident or design. You follow him quickly through the door, knowing that it will be hard to find again if it shuts, all while subtly gripping your weapons. After all, you never can be too careful.

		The door leads into a short corridor designed, you suspect, to prevent light from leaking out into the passage before it opens up into a small, hidden bar dully lit with a few candles. You were completely confident that you knew every drinking establishment in Gloomhaven, but this place is new to you.

		There are a handful of very rough-looking patrons alone or in small groups, who seem to stiffen when the cloaked figure enters. There is no question who is the most important person in the room. Raising a hand towards the barman, your new contact signals for a drink to be brought for you all. You follow the hooded figure to a rough, circular table and are motioned to sit. The cloaked figure pulls back its hood, and you are shocked to discover long, red hair fall to the figure’s shoulders. She forms a wicked, lopsided smile and says, ”Welcome to the Crimson Guild.”

		Your surprise at the Valrath woman’s unveiling obviously gives her great pleasure. “Forgive any subterfuge on my part. I need to protect my secrets,” she says with a sly smile “I am Selandre, and this Guild contains the finest mercenaries, adventurers, and other… persuasive people.”

		“Those in the know come to me for disposal of problems, or acquisition of certain artifacts, like the one you have gathered for me.” She beckons, and you slide the wrapped crystal across to her. Unwrapping it, she smiles again.

		“This is the Frosted Crystal. It will be useful to tackle the Lavalite, one of two particularly unpleasant Savvas who has been causing my benefactors some issues.”

		“But we’ll come to that later,” she says, whipping the crystal off the table and into her robes. ”You’ll earn the bigger jobs in due course. First, I need you to recover some trinkets for a friend of mine. Councilman Raksani collects antiquities, and has notified me of a beast laying golden eggs in the Lingering Swamp. Find and recover three of them, and I’ll make sure you get your fair share of future jobs.”

		You are returning to your normal haunt—The Sleeping Lion—to grab a drink or two for the road, when a door close to the East Wall bursts open. “Help!” croaks an elderly man, “‘Tis the bloody pox! The guards have brought it inside the gates!” With a spluttering cough, he sinks to his knees, vomits blood, and takes his last breath.

		You discuss whether to go and check the guards or travel straight to the Lingering Swamp to recover the eggs for Selandre.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<HydraSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(10),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario004>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario007>()),
	];

	public override string BGMPath => "res://Audio/BGM/Dark-Abyss.ogg";
	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private readonly List<Water> _waterTiles = new List<Water>();
	private readonly List<Hex> _waterSpawnHexes = new List<Hex>();

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<HydraSpirit>()));

		AddScenarioRule(textParameters =>
			$"""
			 At the end of each round, the water tiles marked {Icons.InlineMarker(Marker.Type.a, textParameters)} and all spawned water tiles to the right of them move one hex toward the hexes marked {Icons.InlineMarker(Marker.Type.b, textParameters)}. These water tiles cannot be removed. After every round, a new column of water tiles will spawn to the right of the other columns.
			 """);
		AddScenarioRule(textParameters =>
			$"""
			 When all hexes marked {Icons.InlineMarker(Marker.Type.b, textParameters)} are occupied by water tiles, the scenario is immediately lost.
			 """);

		foreach(Marker marker in GameController.Instance.Map.Markers)
		{
			if(marker.MarkerType == Marker.Type.a)
			{
				_waterSpawnHexes.Add(marker.Hex);
				_waterTiles.Add(marker.GetHexObject<Water>());
			}
		}

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				// Move all water tiles one hex to the left
				foreach(Water water in _waterTiles)
				{
					Hex currentHex = water.Hex;
					Hex newHex = GameController.Instance.Map.GetHex(currentHex.Coords.Add(Direction.West));
					water.TweenGlobalPosition(newHex.GlobalPosition, 0.5f).SetEasing(Easing.OutQuad).PlayFastForwardable();
				}

				await GDTask.DelayFastForwardable(0.5f);

				foreach(Water water in _waterTiles)
				{
					Hex currentHex = water.Hex;
					Hex newHex = GameController.Instance.Map.GetHex(currentHex.Coords.Add(Direction.West));
					water.SetOriginHexAndRotation(newHex);
				}

				// Spawn a new column of water hexes to follow the first column
				for(int i = _waterSpawnHexes.Count - 1; i >= 0; i--)
				{
					Hex newHex = _waterSpawnHexes[i];
					Water newWater =
						await AbilityCmd.CreateDifficultTerrain(newHex,
							ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn")) as Water;
					_waterTiles.Add(newWater);
				}

				if(parameters.RoundIndex == 10)
				{
					// The scenario is lost, the water is all the way to the left
					await AbilityCmd.Lose();
				}
			}
		);
	}
}