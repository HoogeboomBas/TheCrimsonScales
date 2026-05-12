using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario009 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario009.tscn";

	public override int ScenarioNumber => 9;
	public override string Name => "Rescue Mission";

	public override List<ScenarioLink> Links => [new ScenarioLink<Scenario008>()];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override IEnumerable<ScenarioConnection> Connections =>
	[
		new ScenarioConnection<Scenario011>(),
		new ScenarioConnection<Scenario013>(),
		new ScenarioConnection<Scenario014>()
	];

	public override string IntroductionText =>
		"""
		You make your way down a deep set of stairs, which lead to another wooden door. Luckily for you, this one is unlocked. With a brief nod between you, you push the door open. All of a sudden, you’re blinded by flashing lights, and when you manage to squint your eyes open you see real guards—and more guns.

		“You’ve taken a wrong turn, intruder” one of the guards snarls as he slides his fingers across the side of his blade. “This place was built to withstand tougher foes than you.” He whistles and the guns begin to make the clicking sounds again. It doesn’t look like he’s going to give you a chance to explain yourself.
		""";

	public override string ConclusionText =>
		"""
		With the monstrous bear defeated and the guards slain, you turn to the rest of the room. The large, chained chest in the center of the room rocks back and forth as you approach it. Cautiously, you take an axe to the chain. Fortunately, you did not merely cleave the chest in two, as a small, undernourished Quatryl slowly appears, with wide, terrified eyes.

		“Y-you killed Granurso?” he stammers, looking at the remains of the bear. “They said they were going to feed me to him if I didn’t do… this,” he gestures, indicating nervously at the mechanical artillery, his long fingers twitching and his whole body in a state of constant nervous animation. He is clearly deeply traumatized.

		“It’s ok” you reassure him. “You’re safe now. We were sent by Selandre to rescue you.” “Selandre?” replies the Quatryl, brow furrowing. “W-well, you have certainly saved me from a terrible fate here. I don’t know how to thank you.”

		You are considering whether to explain that you are already being paid, but keep your counsel long enough for the animated little creature to scurry over to one of the ruined cannons and fashion a small device from the ruins.

		“Here,” he says, presenting you with his creation. “It’s not much, but I hope you find it useful.” You thank him for his generosity (despite not knowing what the thing is) and take him back to Selandre’s hideout.

		Again the bar is quiet; there are just three Vermlings betting on some kind of game involving carved stones and a couple of big Inox deep in conversation. The bar is a little less gloomy this time, and you can see that the walls and drapes are all in a shabby crimson. You also notice that there’s a faded sign ‘The Crimson Scale’’ behind the bar. The barman Arrok is also behind there, and propping up one end is the Quatryl—who starts to sing again. “Back our new friends come again, From their latest tussle. Killing bears must make you rich, As they’ve gained some hired muscle!”

		Torn between wondering how the singing Quatryl knows about the bear, and where to stick his instrument, you don’t notice Selandre appear from a door behind the bar. “Sankas!” she calls to the Quatryl with a crooked smile, “Long time, no see. I asked my friends to come to your aid as soon as I heard that you were being enslaved in that castle.”

		She turns to you “Did you know that Sankas is one of Gloomhaven’s finest engineers, with a particular talent for designing delightfully inventive weaponry?” You murmur that you have had personal experience of his inventiveness, but also notice that Sankas does not appear totally delighted that Selandre is his savior.

		“Anyway, you look like you could do with a good meal and a debrief, Sankas. Come with me—I have a proposition for you too.”

		As she leaves, she turns back and says to you, “Oh and well done, we’ve sent several other parties to slay Granurso, and none have returned. There’s a gesture of my appreciation on the bar, and I’ll have another little task for you shortly.”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtillery>(),
		ModelDB.Monster<Granurso>(),
		ModelDB.Monster<CityArcher>(),
		ModelDB.Monster<CityGuard>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(10),
		//TODO:new GainCollectiveItemReward(ModelDB.Item<SmogEmitter>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario011>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario013>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario014>()),
	];

	private readonly List<Door> _firstDoors = new List<Door>();
	private ScenarioRule _doorsLockedRule;
	private ScenarioRule _somethingWillHappenRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(revealedOnly: true));
		await AddGoal(new LootGoalTreasuresGoal());

		_doorsLockedRule = AddScenarioRule("The doors are currently locked.");
		_somethingWillHappenRule = AddScenarioRule("Something will happen once all enemies in this room are killed.");

		foreach(Marker marker in GameController.Instance.Map.Markers)
		{
			if(marker.MarkerType == Marker.Type._1)
			{
				_firstDoors.Add(marker.GetHexObject<Door>());
			}
		}

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
			{
				foreach(Figure figure in GameController.Instance.Map.Figures)
				{
					if(figure.Alignment == Alignment.Monsters)
					{
						return false;
					}
				}

				return true;
			},
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

				await SpawnMonster(null, ModelDB.Monster<Granurso>(), MonsterType.Named, [_firstDoors[0].Hex, _firstDoors[1].Hex]);

				foreach(Door door in _firstDoors)
				{
					await door.Unlock();
				}

				_doorsLockedRule.Remove();
				_somethingWillHappenRule.Remove();
			}
		);
	}

	// private async GDTask SpawnBear()
	// {
	// 	MonsterModel monsterModel = ModelDB.Monster<Granurso>();
	// 	MonsterType monsterType = MonsterType.Named;
	//
	// 	Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
	// 		list =>
	// 		{
	// 			// Find the hexes closest to the doors
	// 			List<Hex> hexes = RangeHelper.GetHexesInRange(_firstDoors[0].Hex, RangeHelper.InfiniteRange, false, false).Where(hex => hex.IsEmpty())
	// 				.ToList();
	// 			hexes.Sort((hexA, hexB) => GetMinDistanceToDoor(hexA).CompareTo(GetMinDistanceToDoor(hexB)));
	//
	// 			if(hexes.Count == 0)
	// 			{
	// 				return;
	// 			}
	//
	// 			int minDistance = GetMinDistanceToDoor(hexes[0]);
	//
	// 			foreach(Hex otherHex in hexes)
	// 			{
	// 				int otherDistance = Mathf.Min(RangeHelper.Distance(_firstDoors[0].Hex, otherHex),
	// 					RangeHelper.Distance(_firstDoors[1].Hex, otherHex));
	// 				if(otherDistance > minDistance)
	// 				{
	// 					break;
	// 				}
	//
	// 				list.Add(otherHex);
	// 			}
	// 		}, true, $"Select where to spawn {monsterModel.Name}"
	// 	);
	//
	// 	if(chosenHex == null)
	// 	{
	// 		return;
	// 	}
	//
	// 	await AbilityCmd.SpawnMonster(monsterModel, monsterType, chosenHex);
	// }
	//
	// private int GetMinDistanceToDoor(Hex hex)
	// {
	// 	return Mathf.Min(RangeHelper.Distance(_firstDoors[0].Hex, hex), RangeHelper.Distance(_firstDoors[1].Hex, hex));
	// }
}