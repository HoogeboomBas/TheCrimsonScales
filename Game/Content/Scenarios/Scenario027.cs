using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario027 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario027.tscn";

	public override int ScenarioNumber => 27;
	public override string Name => "Frostbite Cavern";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario028>()];

	public override string IntroductionText =>
		"""
		Flushed with your success with the Lavalite, you return jubilantly to The Crimson Scale and find Selandre waiting for you.

		“Congratulations,” she says. “I gather you’ve completed the first half, now onto the second part.” As she starts to outline the whereabouts of the Icebound’s lair, you start to feel slightly put out that there is no pause to savor your victory.

		Eventually, Selandre picks up on this. “What? Do you want me to throw you a party? Tell you how brave and clever you’ve been? Come on, this is business. You’ve been rewarded for one, now we finish the other one.”

		’We?’ you think, but don’t say, as Selandre continues to tell you how dangerous the Icebound can be.

		Even getting to his lair is treacherous, back up into the mountains, but into a particularly treacherous and windswept area. The cold is biting, and you are glad when you find the Icebound’s cavern, as much to get out of the weather as to confront this monster.

		As you enter the cavern, you see an array of demons and golems in front of you. “We seek the Icebound!” you call out confidently.

		The stone golem replies in a rumbling tone: “Our master does not meet with just any part-time mountain climbers. You must prove your worth.” You reach for your weapons, and notice the Orb of Embers starting to glow. He is close.
		""";

	public override string ConclusionText =>
		"""
		The Icebound jumps, swoops and evades you but, after a lengthy battle, you get the better of him. As you stand over his mortally wounded body, like the Lavalite, he tries to get out some final words. “Think about... who and what... you are fighting for,” he whispers, before the icy glow of his chest dims, his eyes cloud, and his body becomes one with the icy rock of the cavern once more.

		You return to The Crimson Scale, expecting little fanfare after your last triumph, but instead Selandre is jubilant.

		“Well done, well done!” she cries, happier than you’ve ever seen her. “Another piece falls! Drinks all round!”

		You ask what she means, but she doesn’t reply immediately, and then says, “all the powerful, evil influences in Gloomhaven— we’re eliminating them one by one. Someday soon, there will only be one left— and we must be ready for that day.”

		Abruptly her demeanor changes, and she says “I have work to do, I must check on the progress.” And with that, she turns on her heel and strides off. You are finding Selandre more and more unpredictable, but the money keeps on rolling in, so you enjoy the free drinks and celebrate your victory without her.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FrostDemon>(),
		ModelDB.Monster<Icebound>(),
		ModelDB.Monster<StoneGolem>(),
		ModelDB.Monster<WindDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(10),
		new GainCollectiveItemReward(ModelDB.Item<DizzyingTincture>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario028>()),
	];

	private ScenarioRule _scenarioDoorRule;
	private ScenarioRule _scenarioOrbRule;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);
	}

	private Door _door2;

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		Figure orbOfEmbersCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to hold the Orb of Embers");

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters => parameters.Performer == orbOfEmbersCharacter && parameters.AbilityState.Target is Monster monster &&
			              monster.MonsterModel is Icebound,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);
				await GDTask.CompletedTask;
			});

		_scenarioOrbRule.Remove();

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters => parameters.Figure == orbOfEmbersCharacter,
			parameters => parameters.Add(
				new InfoTextExtraEffect.Parameters(textParameters =>
					$"This character holds the Orb of Embers and adds +1{Icons.Inline(Icons.Attack)} to all its attacks against the Icebound.")));
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<Icebound>(), specificCount: 1));

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		_scenarioOrbRule = AddScenarioRule(textParameters =>
			$"""
			 At the beginning of the scenario, nominate one character to hold the Orb of Embers. The Orb of Embers cannot be transferred to another character, and it becomes inactive if the nominated character becomes exhausted.
			 """);

		_scenarioDoorRule = AddScenarioRule(textParameters =>
			$"""
			 Door {Icons.InlineMarker(Marker.Type._2, textParameters)} is currently locked.
			 """);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			await ShowText(
				"""
				As you start to push the door, both it and the other door fly open as driven by a huge draught and, with a freezing gust of wind and snow, the Icebound appears.

				“You step into MY cavern and expect to live?” the monster accuses with a deep, harsh voice. “You’ll need more than that glowing rock to stop me! Who are you? More of Selandre’s minions? I would send you back with a message for her, but I fear that there will be no survivors.”

				He sweeps across the room with the same icy blast, but you’ve come too far to be put off by some strong words and a stiff breeze.
				""");

			await _door2.Unlock();
			await _door2.Open(parameters.PotentialOpener);

			_scenarioDoorRule.Remove();

			AddScenarioRule(textParameters =>
				$"""
				 The character holding the Orb of Embers adds +1{Icons.Inline(Icons.Attack)} to all attacks targeting the Icebound.
				 """);
		}
	}
}