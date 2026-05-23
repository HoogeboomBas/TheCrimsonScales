using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario015 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario015.tscn";

	public override int ScenarioNumber => 15;
	public override string Name => "Ambush of Beasts";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SailScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario016>()];

	public override string IntroductionText =>
		"""
		As you attempt to head towards your ship carrying the glowing power source between you, you find yourself surrounded by wild island animals who look like they’ve been possessed. Again, they are extremely aggressive, and their eyes have an eerie glow that mimics the blue-green of the power source itself. This does not look right, but if you are going to make it back to your ship alive, you’ll have to fight some of the animals off.
		""";

	public override string ConclusionText =>
		"""
		Surrounded by dead bodies of the various creatures that inhabit the island, you swing your blade and the rest of the animals scurry off. Having witnessed the fate of their fellow creatures, they must not want to share the same fate. You board the empty ship and somehow make it back to Gloomhaven (although the harbor master is not too happy with your ‘docking’ attempt).

		As you haul the heavy casket back to The Crimson Scale, you ponder many questions. Who was behind the attempted ambush? What is this power source that so affects animals, and the air itself? And what does Sankas (or Selandre) want with it?

		You are still thinking about these things when you reach The Crimson Scale. Selandre is there, as is the singing Quatryl, though she seems to be a little scared of her and holds his peace. Selandre seems genuinely overjoyed to see you dragging the lead casket with its glowing innards into the room. “Wow! Sankas, come here quickly! They’ve found it!” she calls.

		After a short delay, Sankas emerges from the back room wearing a heavy leather apron and some extraordinary goggles. You have no idea what they are for, but he has clearly been manufacturing something, as he is covered in soot and grime.

		“By the Oak!” he mumbles in awe, “I never thought I’d live to see one of these. I half thought they were a myth, a bed-time story of our history.

		“What does it do?” you ask, intrigued by the reverence with which Sankas is treating this artifact. “I thought it was just a power source?”

		“It is not just a power source,” says Sankas slightly defensively. “This cube contains not only enormous energy, but also a limited power to influence the actions of small beasts and so on.”

		“That would explain the possessed creatures on the island” you mutter.

		“Really?” says Sankas, now extremely attentive “It wanted to stay hidden then… Anyway” he continues “it contains incredible energy, both physically and psionically in its current state, but if those effects could be magnified…” At this, Selandre ushers Sankas away hurriedly, before turning back to you.

		“Thank you—you’ve made a strange little creature very happy!” she says, smiling at the disappearing Sankas. “Did it all go ok?” Selandre asks, sliding a bag of gold your way. You tell her about the ambush and her eyes narrow.

		“You work for me, and the Captain knew that. This wasn’t organized by him. And there were demons?” She grows visibly angrier. “Go back to the docks. Find a ship called The Preto Krisanta. It belongs to the Apex Demon. He was responsible for this.”

		“Make him go away, then come back here—I have another problem with a village’s water supply.”

		For the first time, Selandre looks stressed. Without saying another word, she turns and leaves, sweeping the glasses off the table as she goes.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<RendingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(15),
		new GainRandomOrbEachReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario016>())
	];

	private readonly List<Marker> _markers = new List<Marker>();

	private readonly List<MonsterModel> _monsterSpawnOrder =
	[
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<Lurker>()
	];

	private int _spawnNumber;

	private ScenarioRule _firstRoundRule;
	private ScenarioRule _secondRoundRule;
	private ScenarioRule _laterRoundsRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new SurviveXRoundsScenarioGoal(10, true));

		_firstRoundRule =
			AddScenarioRule(textParameters =>
				$"At the end of the first round, one island creature spawns at {Icons.InlineMarker(Marker.Type.a, textParameters)}.");
		_secondRoundRule =
			AddScenarioRule(textParameters =>
				$"At the end of the second round, one island creature spawns at {Icons.InlineMarker(Marker.Type.c, textParameters)}.");
		_laterRoundsRule =
			AddScenarioRule(textParameters =>
				$"At the end of each round after that, two different island creatures will spawn at {Icons.InlineMarker(Marker.Type.b, textParameters)} and {Icons.InlineMarker(Marker.Type.d, textParameters)} at the end of every odd round, and {Icons.InlineMarker(Marker.Type.a, textParameters)} and {Icons.InlineMarker(Marker.Type.c, textParameters)} at the end of every even round.");

		string spawnRuleText = GameController.Instance.CharacterManager.Characters.Count switch
		{
			2 =>
				"The type of island creature that spawns cycles in order of Hound, Cave Bear, Giant Viper, Rending Drake, and Lurker. All spawns are normal.",
			3 =>
				"The type of island creature that spawns cycles in order of elite Hound, normal Cave Bear, elite Giant Viper, normal Rending Drake, and elite Lurker.",
			4 =>
				"The type of island creature that spawns cycles in order of Hound, Cave Bear, Giant Viper, Rending Drake, and Lurker. All spawns are elite.",
			_ => null
		};
		AddScenarioRule(spawnRuleText);

		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.a));
		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.c));
		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.b));
		_markers.Add(GameController.Instance.Map.GetMarker(Marker.Type.d));

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await SpawnMonster();

				if(parameters.RoundNumber > 2)
				{
					await SpawnMonster();
				}

				if(parameters.RoundNumber == 1)
				{
					_firstRoundRule.Remove();
				}

				if(parameters.RoundNumber == 2)
				{
					_secondRoundRule.Remove();

					_laterRoundsRule.SetText(textParameters =>
						$"At the end of each round, two different island creatures will spawn at {Icons.InlineMarker(Marker.Type.b, textParameters)} and {Icons.InlineMarker(Marker.Type.d, textParameters)} at the end of every odd round, and {Icons.InlineMarker(Marker.Type.a, textParameters)} and {Icons.InlineMarker(Marker.Type.c, textParameters)} at the end of every even round.");
				}
			});
	}

	private MonsterType CalculateMonsterType()
	{
		if(GameController.Instance.SavedCampaign.Characters.Count > 3 ||
		   ((_spawnNumber % 5 & 1) == 0 && GameController.Instance.SavedCampaign.Characters.Count == 3))
		{
			return MonsterType.Elite;
		}

		return MonsterType.Normal;
	}

	private async GDTask SpawnMonster()
	{
		Hex spawnPoint = _markers[_spawnNumber % 4].Hex;
		await SpawnMonster(null, _monsterSpawnOrder[_spawnNumber % 5], CalculateMonsterType(), spawnPoint);
		_spawnNumber++;
	}
}