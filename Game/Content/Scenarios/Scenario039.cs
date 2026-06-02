using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario039 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario039.tscn";

	public override int ScenarioNumber => 39;
	public override string Name => "Festering Mire";

	protected override List<ScenarioRequirement> Requirements => [new PersonalQuestRequirement(ModelDB.PersonalQuest<AnAdderDivides>())];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	public override string IntroductionText =>
		"""
		After trudging through some murky places, you finally are able to piece together the little clues you’ve found, and narrow down the white viper to a region in the Lingering Swamp. You had been searching for days before you finally saw it. A pure white viper; just like you saw on that foreboding day. You’re not mad. And another one. And another... They seem to be living in a massive pit concealed by the overgrowth.

		Suddenly the ground begins to shift, and before you know it the earth beneath your feet collapses and you tumble into the pit below. To make matters worse, the pit appears to be a nesting ground, and disturbing it has aggravated the vipers considerably.

		There is no time to think now, you’ll have to fight your way out. You jump to your feet and ready your weapons. Or; at least you try to. It looks like the fall hurt one of you more than you realized.
		""";

	public override string ConclusionText =>
		"""
		Miraculously finding your way to your feet before you are overwhelmed, you scramble out of the pit as fast as you can, humoring yourself that this is, at worst, only the third worst hole in the ground you’ve fallen into. You manage to kill a few Ghost Vipers during your escape and scrape together some remains to finally prove to everyone that you were right about the pure white viper. You shudder one last time and make your way back to town.

		People are surprised by this new discovery of vipers with such ghastly skin, but within a few days, they are already forgotten by everyone but you. In the end, you can’t help wonder why you went through this whole ordeal.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<GhostViperScenario039>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(10),
		new AddCityToTopQueueReward(ModelDB.Event<City57>()) // TODO: Currently doesn't shuffle this with 5 other cards into the top queue
	];

	private List<Hex> _markerAHexes;
	private Hex _markerBHex;
	private Hex _markerCHex;

	private CustomScenarioGoal _turnsLeftGoal;
	private CustomScenarioGoal _goal;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.SufferDamage(character, 1, character);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_turnsLeftGoal = await AddGoal(new CustomScenarioGoal(
			textParameters => "The Viper Hunter can leave their starting hex after 8 rounds.",
			onStart: async goal =>
			{
				ScenarioEvents.RoundEndedEvent.Subscribe(goal,
					parameters => true,
					async parameters =>
					{
						await goal.AdjustProgress(1);
					}
				);

				await GDTask.CompletedTask;
			},
			hasProgress: true, maxProgress: 8
		));

		_goal = await AddGoal(new CustomScenarioGoal(
			textParameters => $"All characters occupy hexes with {Icons.InlineMarker(Marker.Type.a, textParameters)}.",
			onStart: async goal =>
			{
				ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
					parameters => parameters.Figure is Character,
					async parameters =>
					{
						await goal.SetProgress(
							GameController.Instance.CharacterManager.Characters.Count(character => _markerAHexes.Contains(character.Hex)));
					}
				);

				await GDTask.CompletedTask;
			},
			hasProgress: true,
			maxProgress: GameController.Instance.CharacterManager.Characters.Count
		));

		AddScenarioRule(textParameters =>
			$"If any character is exhausted while not occupying a hex {Icons.InlineMarker(Marker.Type.a, textParameters)}, the scenario is lost.");

		AddScenarioRule(textParameters =>
			$"The character with the An Adder Divides Personal quest is the Viper Hunter. They cannot leave their starting hex in any way until the start of the ninth round.");

		AddScenarioRule(textParameters =>
			$"""
			 Ghost Vipers add +1{Icons.Inline(Icons.Attack)} to each attack targeting a figure that has {Icons.Inline(Icons.GetCondition(Conditions.Poison1), textParameters)}.
			 Whenever a Ghost Viper would give {Icons.Inline(Icons.GetCondition(Conditions.Immobilize), textParameters)} to a figure, that figure does not gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize), textParameters)} and gains {Icons.Inline(Icons.GetCondition(Conditions.Muddle), textParameters)} instead.
			 """);

		ScenarioRule timingRule = AddScenarioRule("Something will happen at the end of the third round.");

		//TODO: Have half the ghost vipers draw a different card

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<VipertoothDagger>());

		_markerAHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex).ToList();
		_markerBHex = GameController.Instance.Map.GetMarker(Marker.Type.b).Hex;
		_markerCHex = GameController.Instance.Map.GetMarker(Marker.Type.c).Hex;

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters =>
				parameters.Performer is Monster monster &&
				monster.MonsterModel is GhostViperScenario039 &&
				parameters.AbilityState.Target.HasPoison(),
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);
				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.InflictConditionEvent.Subscribe(this,
			parameters =>
				parameters.PotentialAbilityState?.Performer is Monster monster &&
				monster.MonsterModel is GhostViperScenario039 &&
				parameters.ConditionModel == Conditions.Immobilize,
			async parameters =>
			{
				parameters.SetPrevented(true);
				await AbilityCmd.AddCondition(parameters.PotentialAbilityState, parameters.Target, Conditions.Muddle);
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character &&
				!_markerAHexes.Contains(parameters.Figure.Hex),
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character character &&
				character.SavedCharacter.SavedPersonalQuest?.Model is AnAdderDivides &&
				GameController.Instance.ScenarioPhaseManager.RoundIndex + 1 < 9,
			parameters =>
			{
				parameters.SetCanEnter(false);
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character character &&
				character.SavedCharacter.SavedPersonalQuest?.Model is AnAdderDivides,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This character is the Viper Hunter (see special rules)."));
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			roundEndedParameters => roundEndedParameters.RoundNumber == 3,
			async roundEndedParameters =>
			{
				ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
					parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1)
						.Any(figure => figure.AlliedWith(parameters.Performer, true) && figure.HasPoison()),
					async parameters =>
					{
						parameters.ForgoAction();

						ActionState actionState = new ActionState(parameters.Performer,
						[
							OtherAbility.Builder()
								.WithPerformAbility(async state =>
								{
									Figure figure = await AbilityCmd.SelectFigure(state, list =>
									{
										list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
											.Where(figure => figure.AlliedWith(parameters.Performer, true) && figure.HasPoison()));
									}, hintText: () => $"Choose a figure to remove {Icons.Inline(Icons.GetCondition(Conditions.Poison1))} from");

									if(figure == null)
									{
										return;
									}

									if(figure.TryGetCondition(Conditions.Poison1, out Condition poison1))
									{
										await AbilityCmd.RemoveCondition(poison1);
									}
									else if(figure.TryGetCondition(Conditions.Poison2, out Condition poison2))
									{
										await AbilityCmd.RemoveCondition(poison2);
									}
									else if(figure.TryGetCondition(Conditions.Poison3, out Condition poison3))
									{
										await AbilityCmd.RemoveCondition(poison3);
									}
									else if(figure.TryGetCondition(Conditions.Poison4, out Condition poison4))
									{
										await AbilityCmd.RemoveCondition(poison4);
									}
								})
								.Build()
						]);
						await actionState.Perform();
					},
					EffectType.Selectable,
					effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Poison1)),
					effectInfoViewParameters: new TextEffectInfoView.Parameters(
						$"Remove {Icons.GetCondition(Conditions.Poison1)} from self or one adjacent ally")
				);

				ScenarioEvents.RoundStartBeforeCardSelectionEvent.Subscribe(this,
					parameters => true,
					async parameters =>
					{
						if(GameController.Instance.SavedCampaign.Characters.Count == 2)
						{
							await SpawnViper(MonsterType.Normal);
							await SpawnViper(MonsterType.Normal);
						}
						else if(GameController.Instance.SavedCampaign.Characters.Count == 3)
						{
							await SpawnViper(MonsterType.Normal);
							await SpawnViper(MonsterType.Normal);
							await SpawnViper(MonsterType.Elite);
						}
						else
						{
							await SpawnViper(MonsterType.Elite);
							await SpawnViper(MonsterType.Elite);
							await SpawnViper(MonsterType.Elite);
						}
					}
				);

				AddScenarioRule(textParameters =>
					$"Any character may forgo a top or bottom action to remove {Icons.Inline(Icons.GetCondition(Conditions.Poison1), textParameters)} from themselves or one adjacent ally.");

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
						AddScenarioRule(textParameters =>
							$"""
							 At the start of each round, spawn one normal and one elite Ghost Viper on the nearest unoccupied hexes to hex {Icons.InlineMarker(Marker.Type.b, textParameters)}.
							 Any time a Ghost Viper would be spawned when there are not enough standees to do so, each character and character summon suffers {Icons.Inline(Icons.Damage, textParameters)}2.
							 """);
						break;
					case 3:
						AddScenarioRule(textParameters =>
							$"""
							 At the start of each round, spawn two normal and one elite Ghost Viper on the nearest unoccupied hexes to hex {Icons.InlineMarker(Marker.Type.b, textParameters)}.
							 Any time a Ghost Viper would be spawned when there are not enough standees to do so, each character and character summon suffers {Icons.Inline(Icons.Damage, textParameters)}2.
							 """);
						break;
					case 4:
						AddScenarioRule(textParameters =>
							$"""
							 At the start of each round, spawn three elite Ghost Vipers on the nearest unoccupied hexes to hex {Icons.InlineMarker(Marker.Type.b, textParameters)}.
							 Any time a Ghost Viper would be spawned when there are not enough standees to do so, each character and character summon suffers {Icons.Inline(Icons.Damage, textParameters)}2.
							 """);
						break;
				}

				timingRule.Remove();
				timingRule = AddScenarioRule("Something will happen at the end of the sixth round.");

				await ShowText(
					"""
					Just when you found time to catch your breath, more Ghost Vipers start slithering into the pit with you. They must have been alerted by the commotion. You’ve got to hold them off until you can help the Viper Hunter back to their feet and get out of this pit.

					“My rucksack!” the Viper Hunter calls to the rest of the party, “grab the anti-venom from it—we’re going to need it before we get out of here.”
					""");

				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

				ScenarioEvents.RoundEndedEvent.Subscribe(this,
					parameters => roundEndedParameters.RoundNumber == 6,
					async parameters =>
					{
						timingRule.Remove();

						ScenarioRule tempRule = AddScenarioRule(textParameters =>
							$"Remove {GameController.Instance.SavedCampaign.Characters.Count} boulder obstacles, and spawn a normal Earth Demon in each of the hexes from which they were removed. If there are not enough boulder obstacles present on the map at this point, each Earth Demon that cannot be spawned in these hexes is instead spawned on hex these hexes is instead spawned on hex {Icons.InlineMarker(Marker.Type.c, textParameters)} and is an elite enemy. If any figure occupies one of the hexes in which a demon should spawn, that figure suffers {Icons.Inline(Icons.Damage, textParameters)}3 and the demon is spawned in the nearest unoccupied hex instead.");

						await ShowText(
							"""
							Just when you thought the situation couldn’t get any worse, you hear a tremendous rumbling and the large rock formations that seemed to shield you from the vipers manifest into hulking giants, displeased by your disturbance here.
							""");

						for(int i = 0; i < GameController.Instance.SavedCampaign.Characters.Count; i++)
						{
							Hex hex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), list =>
									list.AddRange(GameController.Instance.Map.GetChildrenOfType<Boulder1HObstacle>()
										.Where(boulder => !boulder.IsDestroyed)
										.Select(obstacle => obstacle.Hex)), true,
								"Select a boulder to destroy");
							if(hex != null)
							{
								if(hex.IsOccupied())
								{
									await AbilityCmd.SufferDamage(null, hex.GetHexObjectOfType<Figure>(), 3);
								}

								await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Normal, hex);
							}
							else
							{
								if(_markerCHex.IsOccupied())
								{
									await AbilityCmd.SufferDamage(null, _markerCHex.GetHexObjectOfType<Figure>(), 3);
								}

								await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Elite, _markerCHex);
							}
						}

						tempRule.Remove();
					}
				);

				await GDTask.CompletedTask;
			}
		);
	}

	private async GDTask SpawnViper(MonsterType monsterType)
	{
		if(await SpawnMonster(null, ModelDB.Monster<GhostViperScenario039>(), monsterType, _markerBHex, canHaveFeatures: true) == null)
		{
			foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure => figure is Character or Summon))
			{
				await AbilityCmd.SufferDamage(null, figure, 2);
			}
		}
	}
}