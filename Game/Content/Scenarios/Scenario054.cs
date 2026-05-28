using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario054 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario054.tscn";

	public override int ScenarioNumber => 54;
	public override string Name => "Lair of Horrors";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string IntroductionText =>
		"""
		It is another night in the Sleeping Lion, not exactly uneventful: you eject a Vermling who’s trying to pretend an old piece of metal with some runes on is some ancient artifact, and there’s an impressive bar fight, but nothing out of the ordinary. After a couple of hours, and more than a few drinks, an old drunk approaches your table.

		“I used to be like you guys, you know,” he slurs “except maybe better.” You look at the elderly lush, who looks like the only thing he’s explored in the last twenty years is the bottom of a bottle, smile politely and return to your drinks.

		“I don’t look like much now, I know, but in my day there was no-one who could stand in my way. You’d have run me close maybe, but I’d have had your number.” Drunk as this man is, you don’t take being challenged, especially in the Lion.

		“On your way, old man,” you say with a smile, “tell your stories to someone who’ll believe them.” At this, the man seems to sober up and leans in with a hard look on his face. “Listen to me. You think you’re any good? Try stepping in here and coming back with that stupid grin on your face.” He pulls out a scrap of paper and scribbles three words—‘Lair of Horrors’. The next day, having asked around about this ‘Lair of Horrors,’ (although you still have some doubts about the actual level of horror), you find a deep cavern with a small stream flowing gently out of it. It doesn’t look too horrifying. However, as you enter the cave, you begin to see what the old drunk was talking about.

		From one side of the cave there is the familiar clacking of lurkers, though these are a different breed to your normal foes and their darker coloring makes them look more intimidating. There is also a loud hissing noise from above you, and what you had taken to be stalactites move threateningly and reveal themselves as large snakes hanging from the ceiling. Beyond, the cave runs deeper into the mountain face.

		You begin to think that maybe this cave system has earned its name after all...
		""";

	public override string ConclusionText =>
		"""
		Exhausted, you fell the last of the creatures with a howl from both it and you. Catching your breath, you notice by the light of your torch some writing on the back of the cave wall. It says, “Congratulations. Maybe you are as good as I was after all. The next drinks are on me.”

		You gather some of the pelts from the strange animals, which will no doubt fetch a huge sum in the Sinking Market, and head back to Gloomhaven.

		As you walk, questions buzz around your mind. How did that strange man get the message on the back of the cave? How long had it been there? And how did those creatures get there—did they evolve, or were they created?

		You ponder these thoughts all the way back to the Sleeping Lion, where there are indeed drinks waiting for you on the bar.

		As a couple of regulars start to examine the black bear pelt, your questions are largely forgotten by the comforting thought of the beer in your hand and the valuable goods in front of you. Best not to ask too many questions you think, and chalk it down to just another night in The Sleeping Lion.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackBear>(),
		ModelDB.Monster<CaveImp>(),
		ModelDB.Monster<DarkLurker>(),
		ModelDB.Monster<HangingSnake>(),
		ModelDB.Monster<UndeadArcher>(),
		ModelDB.Monster<TerrorDrone>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new GainGoldEachReward(30)
	];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddConditions(null, character, [Conditions.Curse, Conditions.Curse]);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<TrophyHelm>());

		// UpdateScenarioText(
		// 	"""
		// 	The Giant Vipers are Hanging Snakes and have double the number of hit points.
		// 	The Lurkers are Dark Lurkers and use the Harrower Infester monster ability deck instead of their own.
		// 	""");
	}


	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);
		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
		{
// 			UpdateScenarioText(
// 				$"""
// 				 The Living Bones are Undead Archers. Undead Archers gain {Icons.Inline(Icons.Range)}3 and add -1{Icons.Targets} to all their attacks.
// 				 The Stone Golems are Terror Drones and use the Deep Terror monster ability deck instead of their own. Whenever a Terror Drone would summon a Deep Terror, summon an Undead Archer instead.
// 				 """);

			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
				parameters => parameters.Performer is Monster monster && monster.MonsterModel is TerrorDrone &&
				              parameters.AbilityState is MonsterSummonAbility.State summonState && summonState.MonsterModel is DeepTerror,
				async parameters =>
				{
					((MonsterSummonAbility.State)parameters.AbilityState).SetMonsterModel(ModelDB.Monster<UndeadArcher>());
					await GDTask.CompletedTask;
				}
			);

			await ShowText(
				"""
				Battling past these strange variants of beasts, all the more deadly as you’re not quite sure what to expect from them, you find your way into the next chamber.

				As you enter, a volley of arrows greets you. Skeleton archers are positioned around a rocky outcrop, and there is a grinding noise from some even stranger things. You hesitate to call them living, although they move independently, as they seem to consist of towers of rock that have risen up from the cavern floor. Two things are for certain though. They are connected somehow to the Undead Archers, and they are firing stone projectiles at you.
				""");
		}
		else if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
		{
			// UpdateScenarioText(
			// 	"""
			// 	The Cave Bear is the Black Bear and uses the Night Demon monster ability deck instead of their own. All attacks targeting the Black Bear gain Disadvantage.
			// 	The Forest Imps are Cave Imps and have double the number of hit points. Cave Imps use the Ancient Artillery monster ability deck instead of their own.
			// 	""");

			await ShowText(
				"""
				Progressing through to what you hope is the final chamber, the general stench of damp, rot and now warm blood gives way to a new smell. Something animal, but you have the feeling this is no puppy.

				Sure enough, as you get closer an enormous roar comes from the cavern. A rampaging jet-black bear comes at you, teeth bared, with a twinkling array of lights behind him. All too late, you realize that these lights are some kind of cave-dwelling imp, and that they can reach to attack you from afar as the bear rushes to attack you from close in. You prepare for the final battle, hoping with all your heart that this is the last of them.
				""");
		}
	}
}