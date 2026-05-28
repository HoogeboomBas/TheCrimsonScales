using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario037 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario037.tscn";

	public override int ScenarioNumber => 37;
	public override string Name => "Burning Stones";

	protected override List<ScenarioRequirement> Requirements => [new PersonalQuestRequirement(ModelDB.PersonalQuest<NaturalSelection>())];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario038>(true)];

	public override string IntroductionText =>
		"""
		There have been rumors for a while of strange lights, storms of energy and even stranger creatures roaming the edges of the Lingering Swamp, but it doesn’t really catch your attention. However, when an acquaintance disappears, and his travelling companion reports of seeing creatures that can manipulate multiple elements at once, you decide it is time to investigate.

		It is not until evening that you find it—but the reports are true, and it is the elemental lightning that draws you to a rock formation, known as the Burning Stones. You’d always thought that it was just one of those names, long ago lost to history, and now used as a landmark in these featureless marshes, but today the stones burn again.

		You are closest to the Radiant Stone, which is ablaze with light seemingly coming from inside the rock itself. A little further ahead, you see the Flaming Stone—glowing like a hot coal and with sparks and flames spitting from the center. You don’t know how, or why, these rocks are behaving so strangely, but they have attracted the attention of a number of creatures, who seem to gain extra power from being around these mysterious stones.
		""";

	public override string ConclusionText =>
		"""
		You smash the ancient Burning Stones and kill the majority of the creatures, the rest skulking back off into the marshes as the mysterious lights are extinguished and they lose both their boldness and the reflected energy from the mystical stones.

		Although satisfied that you have eliminated what was causing the creatures to grow in power, you remain concerned. None of those demons or Vermlings had the ability to either create or restore the stones with those sorts of powers.

		You study the area to see if you can see anything that might give you a clue as to what caused this, but it is now almost entirely dark.

		You are about to give up when you notice a faint light on the ground. Getting closer, you see that two tunnels have been dug into the soft ground and well propped and lit with torches. Perhaps this will lead you to the cause of the rejuvenated Burning Stones.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<FrostDemon>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<SunDemon>(),
		ModelDB.Monster<VermlingScout>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveRandomStoneReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario038>())
	];

	private Objective _radiantStone;
	private Objective _flamingStone;
	private Objective _shadowStone;
	private Objective _frostStone;
	private Door _door1;

	private CustomScenarioGoal _stonesGoal;

	private ScenarioRule _doorRule;
	private ScenarioRule _radiantStoneRule;
	private ScenarioRule _flamingStoneRule;
	private ScenarioRule _shadowStoneRule;
	private ScenarioRule _frostStoneRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(countObjectives: false));
		_stonesGoal = await AddGoal(new CustomScenarioGoal(textParameters => "Destroy all Burning Stones.",
			hasProgress: true, maxProgress: 4));

		_doorRule = AddScenarioRule(textParameters =>
			$"The door is locked and is unlocked when both Burning Stones are destroyed.");

		_radiantStoneRule = AddScenarioRule(textParameters =>
			$"Until the Radiant Burning Stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Light), textParameters)} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Light), textParameters)}.");

		_flamingStoneRule = AddScenarioRule(textParameters =>
			$"Until the Flaming Burning Stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Fire), textParameters)} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Fire), textParameters)}.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<IronSnare>());

		int burningStoneHealth = GameController.Instance.SavedScenario.ScenarioLevel + GameController.Instance.SavedCampaign.Characters.Count + 2;

		_radiantStone = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
		_radiantStone.Init(burningStoneHealth, "Radiant Burning Stone");

		_flamingStone = GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<Objective>();
		_flamingStone.Init(burningStoneHealth, "Flaming Burning Stone");

		_shadowStone = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();
		_shadowStone.Init(burningStoneHealth, "Shadow Burning Stone");

		_frostStone = GameController.Instance.Map.GetMarker(Marker.Type.d).GetHexObject<Objective>();
		_frostStone.Init(burningStoneHealth, "Frost Burning Stone");

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Objective,
			async parameters =>
			{
				await _stonesGoal.AdjustProgress(1);

				if(parameters.Figure == _radiantStone)
				{
					_radiantStoneRule.Remove();
				}

				if(parameters.Figure == _flamingStone)
				{
					_flamingStoneRule.Remove();
				}

				if(parameters.Figure == _shadowStone)
				{
					_shadowStoneRule.Remove();
				}

				if(parameters.Figure == _frostStone)
				{
					_frostStoneRule.Remove();
				}

				ScenarioEvents.RoundEndedEvent.Unsubscribe(this, parameters.Figure);
				ScenarioCheckEvents.CanConsumeElementCheckEvent.Unsubscribe(this, parameters.Figure);
				if(_radiantStone.IsDestroyed && _flamingStone.IsDestroyed)
				{
					_doorRule.Remove();

					await _door1.Unlock();
				}
			}
		);

		SubscribeStone(_radiantStone, Element.Light);
		SubscribeStone(_flamingStone, Element.Fire);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		_shadowStoneRule = AddScenarioRule(textParameters =>
			$"Until the Shadow Burning Stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Dark), textParameters)} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Dark), textParameters)}.");

		_frostStoneRule = AddScenarioRule(textParameters =>
			$"Until the Frost Burning Stone is destroyed, at the end of each round infuse {Icons.Inline(Icons.GetElement(Element.Ice), textParameters)} and characters cannot consume {Icons.Inline(Icons.GetElement(Element.Ice), textParameters)}.");

		SubscribeStone(_shadowStone, Element.Dark);
		SubscribeStone(_frostStone, Element.Ice);

		await ShowText(
			"""
			You burst through a wall of marshcane growing along a small stream and separating you from the other stones you can see. Further away from you, you can see the Frost Stone emitting a white-blue light with strange, beautiful flames that look like snowflakes around ever growing, upside-down icicles, continually moving and shifting.

			However, the strangest sight is the stone nearest you now—the Shadow Stone.
			""");

		await ShowText(
			"""
			This Stone burns with intensity, but rather than emitting light, somehow seems to be sucking light into it, and the flames too are also barely visible—in fact they look like holes have been cut into a picture where flames should be, except these too are dancing and moving in front of you.

			Again, the Burning Stones have attracted creatures of the dark and ice and inhabitants of the marsh, strengthened and emboldened by this elemental force.
			""");
	}

	private void SubscribeStone(Objective burningStone, Element element)
	{
		ScenarioEvents.RoundEndedEvent.Subscribe(this, burningStone,
			parameters => true,
			async parameters =>
			{
				await AbilityCmd.InfuseElement(null, element, immediately: true);
			}
		);

		ScenarioCheckEvents.CanConsumeElementCheckEvent.Subscribe(this, burningStone,
			parameters => parameters.Figure is Character && parameters.Element == element,
			parameters =>
			{
				parameters.SetCanConsume(false);
			}
		);
	}
}