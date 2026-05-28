using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario006 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario006.tscn";

	public override int ScenarioNumber => 6;
	public override string Name => "Poisoned Water";

	protected override List<ScenarioRequirement> Requirements => [new PartyAchievementRequirement(PartyAchievement.OozeDestroyed, true)];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();

	public override string IntroductionText =>
		"""
		You trace the fountain’s water supply up to the main water source for the whole of Gloomhaven—a large lake at the top of a hill that siphons off into a complex series of pipes and pumps.

		Shiela appears shortly after, straining to push a cart full of crates and barrels. “These will help to purify the water! Careful though, the poisoned water will infect any creatures in the area” she calls from a distance, before letting out a short squeal and disappearing from view.

		You head over to where Shiela was last spotted, splashing through the edge of the lake.
		""";

	public override string ConclusionText =>
		"""
		You finally put enough bottles of antidote into the water to purify it. The town should be protected from the bloody pox, though in the cause of solving the problem, you have managed to find an enemy even more annoying than the usual ooze.

		Shiela emerges carefully from behind the cart. “Thank you,” she stammers, “you have saved me, and the town.” She rummages in her cloak, “Here, take this, it’s the least I can do.”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BloodOoze>(),
		ModelDB.Monster<FlamingDrake>(),
		ModelDB.Monster<ToxicImp>(),
		ModelDB.Monster<WaterSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(2),
		new GainProsperityReward(1),
		new GainCollectiveItemReward(ModelDB.Item<StunPowder>())
	];

	private CustomScenarioGoal _goal;
	private readonly ScenarioRule[] _spawnRules = new ScenarioRule[4];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		int characterCount = GameController.Instance.CharacterManager.Characters.Count;
		_goal = await AddGoal(new CustomScenarioGoal(textParameters =>
			$"Place {characterCount} antidotes in the fountain.", hasProgress: true, maxProgress: characterCount));

		AddScenarioRule("The crate and cabinet obstacles contain the bottles of antidote and cannot be destroyed.");
		AddScenarioRule("Any character may sacrifice the top or bottom action of their turn while adjacent to an antidote to it pick up.");
		AddScenarioRule(
			"Any character may sacrifice the top or bottom action of their turn while adjacent to the fountain to place the antidote in the fountain.");
		AddScenarioRule(
			"Each character may only hold one antidote at a time, and if a character exhausts while holding an antidote, the scenario is immediately lost.");
		AddScenarioRule(
			"Whenever a character picks up an antidote, something will happen.");

		List<Hex> hexesWithAntidote = GameController.Instance.Map.Markers
			.Where(marker => marker.MarkerType == Marker.Type._1)
			.Select(marker => marker.Hex)
			.ToList();

		Hex hexWithFountain = GameController.Instance.Map.Markers
			.First(marker => marker.MarkerType == Marker.Type._2).Hex;

		Dictionary<Figure, bool> characterHasAntidote = [];
		int antidoteBottlesPicked = 0;
		int monsterSpawnsTriggered = 0;

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			characterHasAntidote.Add(character, false);
		}

		object pickSubscriber = new object();
		object placeSubscriber = new object();

		// Allow picking up the antidote
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, pickSubscriber,
			parameters =>
				!parameters.ForgoneAction &&
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex => hexesWithAntidote.Contains(hex) &&
				                                                                    hex.HasHexObjectOfType<Obstacle>()) &&
				!characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
					list =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Where(hex => hexesWithAntidote.Contains(hex)));
					}, mandatory: true);

				parameters.ForgoAction();

				characterHasAntidote[parameters.Performer] = true;

				ScenarioRule newRule = null;
				string text = null;
				switch(antidoteBottlesPicked)
				{
					case 0:
						newRule = new ScenarioRule(textParameters =>
								$"At the end of the round, spawn one elite Blood Ooze closest to the hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)} and one normal Water Spirit closest to the hex marked {Icons.InlineMarker(Marker.Type.b, textParameters)}.",
							-4);
						text =
							"""
							After forcing your way to the head of the track where you last saw Shiela, you retrieve an antidote from the pile of various crates and barrels scattered around her overturned cart. Shiela herself is cowering behind the cart, apparently unharmed but terrified by the Blood Oozes.

							You need to recover the antidote, but also stop the Blood Oozes from getting out of control.
							""";
						break;
					case 1:
						newRule = new ScenarioRule(textParameters =>
								$"At the end of the round, spawn one normal Flaming Drake closest to the hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)} and one normal Flaming Drake closest to the hex marked {Icons.InlineMarker(Marker.Type.b, textParameters)}.",
							-3);
						text =
							"""
							“Watch out!” you hear Shiela cry out from the corner. “Look, up from the sky!”
							""";
						break;
					case 2:
						newRule = new ScenarioRule(textParameters =>
								$"At the end of the round, spawn one normal and one elite Toxic Imp closest to the hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)} and one normal and one elite Toxic Imp closest to the hex marked {Icons.InlineMarker(Marker.Type.b, textParameters)}.",
							-2);
						text =
							"""
							You hear ruffling from the bushes and trees, and all of a sudden a flock of imps burst out from the foliage. They have a purple foam dripping from their mouths and their wings are splattered with blood. They don’t appear to be happy with your presence and you ready yourself to ward them off.
							""";
						break;
					case 3:
						newRule = new ScenarioRule(textParameters =>
								$"At the end of the round, spawn one elite Contaminated Water Spirit closest the hex marked {Icons.InlineMarker(Marker.Type.a, textParameters)} and one elite Contaminated Water Spirit closest to the hex marked {Icons.InlineMarker(Marker.Type.b, textParameters)}.",
							-1);
						text =
							"""
							As you grab the last antidote, the water begins to bubble and boil around you. Several Contaminated Water Spirits emerge from the water and lunge toward you. This isn’t over yet.
							""";
						break;
					default:
						Log.Error("Picked up an antidote to many somehow!");
						return;
				}

				AddScenarioRule(newRule);
				_spawnRules[antidoteBottlesPicked] = newRule;

				await ShowText(text);

				antidoteBottlesPicked++;

				await chosenHex.GetHexObjectOfType<Obstacle>().Destroy(false, true);
				hexesWithAntidote.Remove(chosenHex);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(parameters.Performer, this,
					infoParameters => infoParameters.Figure == parameters.Performer,
					infoParameters =>
					{
						infoParameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
							$"This character is carrying an antidote bottle."));
					}
				);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Pick up a bottle of antidote.")
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				for(int i = monsterSpawnsTriggered; i < antidoteBottlesPicked; i++)
				{
					await SpawnMonsters(i);
					monsterSpawnsTriggered++;
				}
			}
		);

		// Allow placing the antidote into the fountain
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, placeSubscriber,
			parameters => !parameters.ForgoneAction &&
			              RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex => hexWithFountain == hex) &&
			              characterHasAntidote[parameters.Performer],
			async parameters =>
			{
				await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), list => list.Add(hexWithFountain), mandatory: true);
				parameters.ForgoAction();
				characterHasAntidote[parameters.Performer] = false;
				//antidoteBottlesPlaced++;
				await _goal.AdjustProgress(1);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, this);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.StartHexMove),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Place the bottle of antidote in the fountain.")
		);

		// If a character exhausts while holding an antidote, the scenario is immediately lost
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Character && characterHasAntidote[parameters.Figure],
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);
	}

	private async GDTask SpawnMonsters(int spawnIndex)
	{
		Hex hexA = GameController.Instance.Map.Markers.First(marker => marker.MarkerType == Marker.Type.a).Hex;
		Hex hexB = GameController.Instance.Map.Markers.First(marker => marker.MarkerType == Marker.Type.b).Hex;

		switch(spawnIndex)
		{
			case 0: // 6G
			{
				await SpawnMonster(null, ModelDB.Monster<BloodOoze>(), MonsterType.Elite, hexA);
				await SpawnMonster(null, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Normal, hexB);
				break;
			}
			case 1: // 6D
			{
				await SpawnMonster(null, ModelDB.Monster<FlamingDrake>(), MonsterType.Normal, hexA);
				await SpawnMonster(null, ModelDB.Monster<FlamingDrake>(), MonsterType.Normal, hexB);
				break;
			}
			case 2: // 6F
			{
				await SpawnMonster(null, ModelDB.Monster<ToxicImp>(), MonsterType.Normal, hexA);
				await SpawnMonster(null, ModelDB.Monster<ToxicImp>(), MonsterType.Elite, hexA);
				await SpawnMonster(null, ModelDB.Monster<ToxicImp>(), MonsterType.Normal, hexB);
				await SpawnMonster(null, ModelDB.Monster<ToxicImp>(), MonsterType.Elite, hexB);
				break;
			}
			case 3: // 6E
			{
				await SpawnMonster(null, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Elite, hexA);
				await SpawnMonster(null, ModelDB.Monster<ContaminatedWaterSpirit>(), MonsterType.Elite, hexA);
				break;
			}
		}

		_spawnRules[spawnIndex].Remove();
	}
}