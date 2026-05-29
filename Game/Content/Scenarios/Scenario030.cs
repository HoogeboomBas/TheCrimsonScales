using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario030 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario030.tscn";

	public override int ScenarioNumber => 30;
	public override string Name => "Undead Terrors";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario032>()];

	public override string IntroductionText =>
		"""
		You crawl through the narrow hole you have formed from the previous chamber, into yet another warren of vaults. The undead are on you immediately, but this feels different, almost like they are protecting something. You are on the right path. Tired, but battle ready from your previous exertions you steel yourself for one more push. Time to get to the bottom of who, or what, is responsible for this necromancy.
		""";

	public override string ConclusionText =>
		"""
		You stand bloodied and battered, a pile of bones at your feet and the glowing font in front of you. Unsure how to dispose of the potion, you first carefully take a vial—Shiela will be interested in this, no doubt—before trying to work out how to destroy the rest. Eventually, you decide to dilute it with all the water you are carrying, hoping that its lack of potency will render it useless. On your last collective flask the liquid loses the last of its luminescence, and you are sure you can hear it almost sigh as a small cloud of vapor floats upwards before the now virtually clear liquid is still.

		You have absolutely no plans to drink it, but it looks as though it has lost its potency. Gathering the papers detailing how to make the potion, you thoroughly destroy all entrances to the chamber to ensure nobody else stumbles across this catacomb of evil.

		You are making your way back to the Guild Hall, when Dominic’s last words to you echo louder and louder in your head. “Remember, destroy the potion. You must destroy it!” With some regret, you pour away the vial you had saved for Shiela, but then your eyes fall on the detailed instructions. What else had Dominic said?

		Don’t let the potion fall into the wrong hands.

		Can you trust Selandre? But, 70 gold each is a lot of money, and that’s what you’re here for.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<ShadowDemon>(),
		ModelDB.Monster<StoneGolem>(),
		ModelDB.Monster<TwinCorpse>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario032>())
	];

	private Door _door2;

	private ScenarioRule _doorRule;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<ShadowDemon>()));
		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<TwinCorpse>(), specificCount: 2));

		_doorRule = AddScenarioRule(textParameters =>
			$"Door {Icons.InlineMarker(Marker.Type._2, textParameters)} is locked until door {Icons.InlineMarker(Marker.Type._1, textParameters)} has been opened.");

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 17).SetItemLoot(ModelDB.Item<ConcussionMine>());
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 38).SetItemLoot(ModelDB.Item<WarPick>());
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			await _door2.Unlock();

			_doorRule.Remove();

			await ShowText(
				"""
				Bursting into the room, you see a large, dark demon, his hands glowing green, coated with the now familiar liquid. He screams in disgusted recognition as he realizes you’ve come to thwart his plans before composing himself. “You’re too late!” he taunts. “My guards are in position, the undead army will soon be here. You can’t destroy my work, and you can’t destroy ME!” With a crazed look on his face, he and his minions launch themselves at you
				""");
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			int summonCount = GameController.Instance.SavedCampaign.Characters.Count + 2;

			AddScenarioRule(
				$"Whenever a Twin Corpse is killed, summon {summonCount} normal Living Corpses in unoccupied hexes nearest to the hex in which it was killed.");

			await ShowText(
				"""
				You open the door to see a font containing the glowing, green potion, and associated ingredients and instructions. The Shadow Demon is dead, and this is what you came for. Guarding the font are two massive animated corpses, no doubt tasked with ensuring that none would leave with the potion alive. Knowing that, fatigued as you are, you must gain those instructions and destroy the potion, and stand ready for one last stand.
				""");

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				figureKilledParameters => figureKilledParameters.Figure is Monster monster && monster.MonsterModel is TwinCorpse,
				async figureKilledParameters =>
				{
					for(int monsterIndex = 0; monsterIndex < summonCount; monsterIndex++)
					{
						await SummonMonster(null, ModelDB.Monster<LivingCorpse>(), MonsterType.Normal, figureKilledParameters.Figure.Hex);
					}
				}
			);
		}
	}
}