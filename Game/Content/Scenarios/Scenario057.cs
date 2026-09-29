using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario057 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario057.tscn";

	public override int ScenarioNumber => 57;
	public override string Name => "Elemental Converter";
	public override ClassModel ClassModel => ModelDB.Class<BrightsparkModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<BrightsparkModel>())];

	public override string IntroductionText =>
		"""
		As you connect the last coupler, you hear a crash in a back room. Peering up from your workbench you realize some of the creatures you’ve kept for observational purposes have broken loose. You begin to rue the terrible timing, what a disaster to happen just as you finish your latest invention.

		Just then an idea comes to my mind. You still have to run the elemental converter through a series of rigorous tests to prove its efficacy. This wasn’t the originally planned time table, but you must always be prepared to seize opportunity when it presents itself.

		You make one final adjustment to the converter, tuck it within your lab coat, and rush off to the side wings of your laboratory, determined to collect a robust set of data.
		""";

	public override string ConclusionText =>
		"""
		You look about yourself. Your laboratory is in shambles with shelves overturned and equipment broken. But you breathe a sigh of relief. The Elemental Converter worked like a charm. The monsters that escaped are dead, and you’re not. A stunning result really.

		With a hefty amount of data to sort through you turn a spilled over table upright, dust it off, and grab a chair. Cleaning up can come later, for now you want to apply what you’ve learned and make the final changes to your invention.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<TestTube>())
	];

	private readonly List<ScenarioRule> _scenarioRules = [];
	private readonly List<string> _roomTiles = ["B2b", "A1a", "B3b", "A2a"];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		SubscribeElement(Element.Ice, 1);
		SubscribeElement(Element.Air, 2);
		SubscribeElement(Element.Light, 3);
		SubscribeElement(Element.Fire, 4);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		foreach(ScenarioRule rule in _scenarioRules.Where(rule => !rule.Removed))
		{
			rule.Remove();
		}

		int index = GameController.Instance.Map.Rooms.IndexOf(roomRevealedParameters.Room);
		AddScenarioRule(_scenarioRules[index - 1]);
	}

	private void SubscribeElement(Element element, int roomNumber)
	{
		ScenarioEvents.ElementInfusedEvent.Subscribe(this, element,
			parameters => parameters.Element == element && parameters.PotentialInfuser is Character character &&
			              character.Hex.Room == GameController.Instance.Map.Rooms[roomNumber],
			async parameters =>
			{
				foreach(Figure figure in GameController.Instance.Map.Rooms[roomNumber].Figures.Where(figure => figure is Monster))
				{
					await AbilityCmd.SufferDamage(null, figure, 2, parameters.PotentialInfuser);
				}
			});

		_scenarioRules.Add(new ScenarioRule(textParameters =>
			$"Each time you generate {Icons.Inline(Icons.GetElement(element), textParameters)} while on the {_roomTiles[roomNumber - 1]} tile, all monsters in the room immediately suffer {Icons.Inline(Icons.Damage, textParameters)}2."));
	}
}