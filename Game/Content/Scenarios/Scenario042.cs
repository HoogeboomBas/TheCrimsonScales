using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario042 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario042.tscn";

	public override int ScenarioNumber => 42;
	public override string Name => "Unnatural Beasts";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Intrigued by the reports of the strange beasts the tiger-riding Orchid told you about, you approach the area of the forest she directed you to. At first glance, all seems clear and calm, but then you notice the eyes.

		From two small gaps in the forest you sense a range of animals watching you, and they slowly begin to growl and hiss. A similar noise comes from behind you, where you see more red eyes and a low, threatening growl. You stand back to back, waiting for the onrushing animals, but none appear. It looks like you’ll have to go to them.
		""";

	public override string ConclusionText =>
		"""
		As you kill the final beast, you retreat back onto the path. The remaining creatures, still dangerous, but not in the same league as the three leaders, are even keener to remain in their respective habitats, and you have no desire to go back in there. You have helped the Orchid’s village by killing the most vicious; they can take care of the rest.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<GoringGrizzly>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<KingCobra>(),
		ModelDB.Monster<SlyWolf>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new GainXPReward(10)
	];

	private Door _door1;
	private Door _door2;
	private Door _door3;
	private IEnumerable<Marker> _markersA;
	private IEnumerable<Marker> _markersB;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<GoringGrizzly>()));
		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<KingCobra>()));
		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<SlyWolf>()));

		AddScenarioRule(textParameters =>
			$"Character and character summons cannot attack while occupying the G1A tile, but may perform attacks while occupying a door hex.");
		AddScenarioRule(textParameters =>
			$"Any character may forgo a top or bottom action to perform “{Icons.Inline(Icons.Push, textParameters)}2, Target one adjacent enemy”.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<WovenPlateArmor>());
		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async lootingCharacter =>
		{
			await AbilityCmd.GainGold(lootingCharacter, 25);
			await AbilityCmd.AddCondition(null, lootingCharacter, Conditions.Poison1);
		});
		GameController.Instance.Map.Treasures[2].SetItemLoot(ModelDB.Item<SteelHelmet>());

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
		_door2 = marker2.GetHexObject<Door>();

		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
		_door3 = marker3.GetHexObject<Door>();

		_markersA = GameController.Instance.Map.GetMarkers(Marker.Type.a);
		_markersB = GameController.Instance.Map.GetMarkers(Marker.Type.b);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters => parameters.AbilityState is AttackAbility.State &&
			              (parameters.Performer is Character || parameters.Performer is Summon) &&
			              GameController.Instance.Map.Rooms[0].MapTiles.Contains(parameters.Performer.Hex.MapTile),
			async parameters =>
			{
				parameters.AbilityState.SetBlocked();

				await GDTask.CompletedTask;
			});

		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1)
				.Any(figure => figure.EnemiesWith(parameters.Performer)),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [
					PushAbility.Builder()
						.WithPush(2)
						.WithRange(1)
						.Build()
				]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Push),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform {Icons.Inline(Icons.Push)}2, {Icons.Inline(Icons.Range)}1")
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.OpenedDoor == _door1)
		{
			List<ScenarioRule> wolfRules = new List<ScenarioRule>();
			wolfRules.Add(AddScenarioRule(textParameters =>
				$"The Sly Wolf is permanently {Icons.Inline(Icons.GetCondition(Conditions.Invisible), textParameters)}."));
			AddScenarioRule(textParameters =>
				$"None of the monsters in the room will focus on enemies outside the D2B tile, nor will they leave the tile.");
			AddScenarioRule(textParameters =>
				$"Whenever a Hound is killed, spawn another Hound in one of the hexes marked {Icons.InlineMarker(Marker.Type.a, textParameters)} at the end of the round.");
			wolfRules.Add(AddScenarioRule(textParameters =>
				$"Whenever a Hound is killed, the Sly Wolf suffers {Icons.Inline(Icons.Damage, textParameters)}{GameController.Instance.CharacterManager.Characters.Count}."));

			Figure slyWolf = GameController.Instance.Map.Figures
				.First(figure => figure is Monster monster && monster.MonsterModel is SlyWolf);

			int houndsToSpawn = 0;
			ScenarioEvents.FigureKilledEvent.Subscribe(this, _door1,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster &&
					monster.MonsterModel == ModelDB.Monster<Hound>() &&
					!slyWolf.IsDead,
				async applyParameters =>
				{
					await AbilityCmd.SufferDamage(slyWolf,
						GameController.Instance.SavedCampaign.Characters.Count, slyWolf);
					houndsToSpawn++;
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, slyWolf,
				canApplyParameters =>
					canApplyParameters.Figure == slyWolf,
				async applyParameters =>
				{
					foreach(ScenarioRule wolfRule in wolfRules)
					{
						wolfRule.Remove();
					}

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.RoundEndedEvent.Subscribe(this, _door1,
				canApplyParameters =>
					houndsToSpawn > 0,
				async applyParameters =>
				{
					while(houndsToSpawn > 0)
					{
						await SpawnMonster(null, ModelDB.Monster<Hound>(), MonsterType.Normal, _markersA.Select(marker => marker.Hex));
						houndsToSpawn--;
					}
				}
			);

			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door1,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster && monster.MonsterModel is Hound &&
					!GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCanEnter(false);
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door1,
				canApplyParameters =>
					canApplyParameters.Performer is Monster monster && monster.MonsterModel is Hound &&
					!GameController.Instance.Map.Rooms[1].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCannotBeFocused();
				}
			);

			await ShowText(
				"""
				You stand at the mouth of the cave, and throw a torch into the center so you can see whatever is in there. It appears to be a pack of wolves, and you catch a glimpse of the leader—a huge ferocious looking beast—before it somehow melts into the shadows at the back of the cave and you lose sight of it. While you are squinting trying to see what happened to the big one, the other wolves circle round the edge of the cave, ready to attack.
				""");
		}
		else if(parameters.OpenedDoor == _door2)
		{
			Figure goringGrizzly = GameController.Instance.Map.Figures
				.First(figure => figure is Monster monster && monster.MonsterModel is GoringGrizzly);

			List<ScenarioRule> grizzlyRules = new List<ScenarioRule>();
			AddScenarioRule(textParameters =>
				$"None of the monsters in the room will focus on enemies outside the D1B tile, nor will they leave the tile.");
			AddScenarioRule(textParameters =>
				$"Whenever a Cave Bear is killed, spawn another Cave Bear in one of the hexes marked {Icons.InlineMarker(Marker.Type.b, textParameters)} at the end of the round.");
			grizzlyRules.Add(AddScenarioRule(textParameters =>
				$"Whenever a Cave Bear is killed, reduce the Shield value of the Goring Grizzly by 1."));

			int caveBearsToSpawn = 0;
			int shieldValue = 3;
			ScenarioEvents.FigureKilledEvent.Subscribe(this, _door2,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster &&
					monster.MonsterModel == ModelDB.Monster<CaveBear>() &&
					!goringGrizzly.IsDead,
				async applyParameters =>
				{
					if(shieldValue > 0)
					{
						shieldValue--;
						ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
					}

					caveBearsToSpawn++;

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.FigureKilledEvent.Subscribe(this, goringGrizzly,
				canApplyParameters =>
					canApplyParameters.Figure == goringGrizzly,
				async applyParameters =>
				{
					foreach(ScenarioRule grizzlyRule in grizzlyRules)
					{
						grizzlyRule.Remove();
					}

					await GDTask.CompletedTask;
				}
			);

			ScenarioCheckEvents.ShieldCheckEvent.Subscribe(this, _door2,
				canApplyParameters =>
					canApplyParameters.Figure == goringGrizzly,
				applyParameters =>
				{
					applyParameters.AdjustShield(shieldValue);
				}
			);

			ScenarioEvents.SufferDamageEvent.Subscribe(this, _door2,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster &&
					monster.MonsterModel is GoringGrizzly &&
					canApplyParameters.FromAttack,
				async applyParameters =>
				{
					applyParameters.AdjustShield(shieldValue);
					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.RoundEndedEvent.Subscribe(this, _door2,
				canApplyParameters =>
					caveBearsToSpawn > 0,
				async applyParameters =>
				{
					while(caveBearsToSpawn > 0)
					{
						await SpawnMonster(null, ModelDB.Monster<CaveBear>(), MonsterType.Normal, _markersB.Select(marker => marker.Hex));
						caveBearsToSpawn--;
					}
				}
			);

			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door2,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster && monster.MonsterModel is CaveBear &&
					!GameController.Instance.Map.Rooms[2].MapTiles.Contains(canApplyParameters.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCanEnter(false);
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door2,
				canApplyParameters =>
					canApplyParameters.Performer is Monster monster && monster.MonsterModel is CaveBear &&
					!GameController.Instance.Map.Rooms[2].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCannotBeFocused();
				}
			);

			await ShowText(
				"""
				Pushing through the first gap in the trees, you find that the deep growling came from bears. There seems to be a clear leader, even more powerful looking and aggressive than a usual bear—which is plenty aggressive enough. As you enter the clearing, it is as if a line has been crossed. You are on their territory now, and they are ready to attack.
				""");
		}
		else if(parameters.OpenedDoor == _door3)
		{
			Figure kingCobra = GameController.Instance.Map.Figures
				.First(figure => figure is Monster monster && monster.MonsterModel is KingCobra);

			List<ScenarioRule> cobraRules = new List<ScenarioRule>();
			AddScenarioRule(textParameters =>
				$"None of the monsters in the room will focus on enemies outside the C2A tile, nor will they leave the tile.");
			cobraRules.Add(AddScenarioRule(textParameters =>
				$"Whenever a Giant Viper is damaged, the King Cobra suffers an equal amount of {Icons.Inline(Icons.Damage, textParameters)}"));

			ScenarioEvents.FigureKilledEvent.Subscribe(this, kingCobra,
				canApplyParameters =>
					canApplyParameters.Figure == kingCobra,
				async applyParameters =>
				{
					foreach(ScenarioRule cobraRule in cobraRules)
					{
						cobraRule.Remove();
					}

					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.AfterSufferDamageEvent.Subscribe(this, _door3,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster &&
					monster.MonsterModel == ModelDB.Monster<GiantViper>() &&
					!kingCobra.IsDead,
				async applyParameters =>
				{
					await AbilityCmd.SufferDamage(kingCobra, applyParameters.Damage, kingCobra);
				}
			);

			ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this, _door3,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster && monster.MonsterModel is GiantViper &&
					!GameController.Instance.Map.Rooms[3].MapTiles.Contains(canApplyParameters.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCanEnter(false);
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, _door3,
				canApplyParameters =>
					canApplyParameters.Performer is Monster monster && monster.MonsterModel is GiantViper &&
					!GameController.Instance.Map.Rooms[3].MapTiles.Contains(canApplyParameters.PotentialTarget.Hex.MapTile),
				applyParameters =>
				{
					applyParameters.SetCannotBeFocused();
				}
			);

			await ShowText(
				"""
				As you enter the next clearing, a spine-chilling hissing erupts from the clearing, and you see several giant snakes, with one in particular towering above the rest. Its enormous cobra’s hood is flared as it rises up, ready to strike. The others follow its lead, and you ready your weapons, hoping you can strike first.
				""");
		}
	}
}