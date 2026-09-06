using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario024 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario024.tscn";

	public override int ScenarioNumber => 24;
	public override string Name => "Eerie Grotto";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario026>(true)];

	public override string IntroductionText =>
		"""
		The Aesther’s notes on the frozen creatures she has discovered are slightly hit and miss — on one hand, the directions are generally very good, but the details of what is to be found within are not so detailed. It is almost as if she needed to record the location, but instinctively knows how to deal with what she finds within.

		The instructions here are headed “Grotto”, before a lengthy description of how to find the place. After which is written the note “Keep Globes away from Dome—will destroy creatures? Beware frosted sections.”

		You don’t understand the reference to the ‘frosted sections’, but a clue as to how to destroy these creatures is too good to pass up...
		""";

	public override string ConclusionText =>
		"""
		One by one you place the globes in the dome, the spheres pulling against your reluctance to give them up. As you let go of the final one, they melt together into a strange liquid, before vaporizing into a strong smelling cloud. The remaining monsters scream in pain as the cloud touches them and dissolve into the icy cavern, which is also melting away as the cloud spreads.

		You don’t really understand what is happening, but realize that now would be a good time to leave before the whole place collapses.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<FrozenCadaver>(),
		ModelDB.Monster<HailDemon>(),
		ModelDB.Monster<HarrowerIcecrawlers>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainPartyAchievementReward(PartyAchievement.FrozenWarrior),
		new GainRandomOrbEachReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario026>())
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private Door _door1;
	private Door _door2;
	private Door _door3;
	private List<Marker> _markers = [];
	private Marker _markerA;
	private Marker _markerB;
	private Marker _markerC;
	private Marker _markerD;
	private List<HazardousTerrain> _hotCoals;
	private Obstacle _dome;
	private int _orbsPlaced = 0;
	private Dictionary<Figure, Marker> _charactersWithOrbs = new Dictionary<Figure, Marker> { };

	private CustomScenarioGoal _goal;
	private ScenarioRule _warmFiresRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(textParameters =>
				$"Place {GameController.Instance.SavedCampaign.Characters.Count} Orbs in the dome.",
			hasProgress: true, maxProgress: GameController.Instance.SavedCampaign.Characters.Count));

		AddScenarioRule(textParameters =>
			$"Any character may forgo the top or bottom action of their turn to remove all {Icons.InlineCondition(Conditions.Chill, textParameters)} tokens from self or one summon they own within {Icons.Inline(Icons.Range, textParameters)}2.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<CursebloodBlade>());

		_markerA = GameController.Instance.Map.GetMarker(Marker.Type.a);

		_markerB = GameController.Instance.Map.GetMarker(Marker.Type.b);

		_markerC = GameController.Instance.Map.GetMarker(Marker.Type.c);

		_markerD = GameController.Instance.Map.GetMarker(Marker.Type.d);

		_markers.AddRange([_markerA, _markerB, _markerC, _markerD]);

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
		_door2 = marker2.GetHexObject<Door>();

		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
		_door3 = marker3.GetHexObject<Door>();

		_hotCoals = GameController.Instance.Map.GetChildrenOfType<HazardousTerrain>();


		List<Obstacle> obstacles = GameController.Instance.Map.GetChildrenOfType<Obstacle>();
		_dome = obstacles[^1];

		//Scenario Win Condition
		// ScenarioEvents.RoundEndedEvent.Subscribe(this,
		// 	parameters => _orbsPlaced == GameController.Instance.SavedCampaign.Characters.Count,
		// 	async parameters =>
		// 	{
		// 		await ((CustomScenarioGoals)ScenarioGoals).Win();
		// 	}
		// );

		//Remove Chill forgo action
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
				.Any(figure => figure.HasCondition(Conditions.Chill) &&
				               ((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)),
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [
					OtherAbility.Builder()
						.WithPerformAbility(async state =>
						{
							Figure figure = await AbilityCmd.SelectFigure(state, list =>
							{
								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 2)
									.Where(figure =>
										(figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure));
							});

							if(figure == null)
							{
								return;
							}

							await AbilityCmd.RemoveCondition(figure, Conditions.Chill, state);
						})
						.Build()
				]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one of your summons within {Icons.Inline(Icons.Range)} 2.")
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.OpenedDoor == _door1)
		{
			AddScenarioRule(textParameters =>
				$"""
				 While adjacent to the hexes marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, {Icons.InlineMarker(Marker.Type.b, textParameters)}, {Icons.InlineMarker(Marker.Type.c, textParameters)}, {Icons.InlineMarker(Marker.Type.d, textParameters)}, each character may forgo the top or bottom action of their turn to pick up the letter representing each Orb and gain the following bonus:
				 {Icons.InlineMarker(Marker.Type.a, textParameters)}: Add +1{Icons.Inline(Icons.Move, textParameters)} to all your move abilities.
				 {Icons.InlineMarker(Marker.Type.b, textParameters)}: Add {Icons.Inline(Icons.Pierce, textParameters, true)} 2 to all your attack abilities.
				 {Icons.InlineMarker(Marker.Type.c, textParameters)}: You are unaffected by {Icons.Inline(Icons.Retaliate, textParameters)}.
				 {Icons.InlineMarker(Marker.Type.d, textParameters)}: Add {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)} to all your attack abilities.
				 """);

			AddScenarioRule(textParameters =>
				"""
				Each character may only hold a maximum of one Orb. If any character exhausts while holding an orb, the scenario is lost.
				""");

			AddScenarioRule(textParameters =>
				"""
				If any character exhausts while holding an Orb, the scenario is immediately lost.
				""");

			AddScenarioRule(textParameters =>
				$"""
				 While occupying the K1b tile, at the end of each character and character summons turn, if they have no {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)} tokens they gain {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)}.
				 """);

			_warmFiresRule = AddScenarioRule(textParameters =>
				$"""
				 The Hot Coal hexes represents Warm Fires and cannot be removed. If a character ends their turn within {Icons.Inline(Icons.Range, textParameters)} 1 of a Warm Fire, they ignore the effect above.
				 """);

			await ShowText(
				"""
				Having dispatched the monsters in the entrance to the grotto, you open a rudimentary doorway set into the rock. Immediately an icy blast hits you, snatching your breath away. To say it is cold here is an understatement.

				You also see the globes, mentioned in the notes you found. Each glowing a different color, they have a strange attracting force, almost pulling you towards them. There are also several more beasts here—who you do not quite feel the same magnetic attraction towards...
				""");

			//lose if character exhausts with an orb
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				canApplyParameters =>
					canApplyParameters.Figure is Character character &&
					_charactersWithOrbs.ContainsKey(character),
				async parameters =>
				{
					await AbilityCmd.Lose();
				}
			);

			//Gain chill at end of round
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _door1,
				canApplyParameters =>
				{
					return canApplyParameters.Figure is Character or Summon;
				},
				async applyParameters =>
				{
					if(GameController.Instance.Map.Rooms[1].Hexes.Contains(applyParameters.Figure.Hex) &&
					   !applyParameters.Figure.HasCondition(Conditions.Chill) &&
					   !RangeHelper.GetHexesInRange(applyParameters.Figure.Hex, 1).Any(hex => hex.HexObjects.Any(obj => _hotCoals.Contains(obj))))
					{
						await AbilityCmd.AddCondition(null, applyParameters.Figure, Conditions.Chill);
					}
				}
			);

			//Forgo action to take an orb
			ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, _door1,
				parameters => !parameters.ForgoneAction && RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex =>
					_markers.Any(marker => marker.Hex == hex)
					&& !_charactersWithOrbs.ContainsKey(parameters.Performer)),
				async parameters =>
				{
					parameters.ForgoAction();

					ActionState actionState = new ActionState(parameters.Performer, [
						OtherAbility.Builder()
							.WithPerformAbility(async state =>
							{
								await AbilityCmd.GenericChoice(parameters.Performer,
								[
									ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters =>
											GameController.Instance.Map.GetMarker(Marker.Type.a) != null &&
											RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Contains(_markerA.Hex),
										applyFunction: async applyParameters =>
										{
											SubscribeToMarkerA(parameters.Performer);
											GameController.Instance.Map.Markers.Remove(_markerA);
											_markers.Remove(_markerA);
											_markerA.Hide();
											_charactersWithOrbs.Add(parameters.Performer, _markerA);
											await GDTask.CompletedTask;
										},
										effectButtonParameters: new IconEffectButton.Parameters(Icons.GetMarker(Marker.Type.a)),
										effectInfoViewParameters: new TextEffectInfoView.Parameters(
											$"Take Orb {Icons.Inline(Icons.GetMarker(Marker.Type.a))}."),
										effectType: EffectType.Selectable
									),
									ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters =>
											GameController.Instance.Map.GetMarker(Marker.Type.b) != null &&
											RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Contains(_markerB.Hex),
										applyFunction: async applyParameters =>
										{
											SubscribeToMarkerB(parameters.Performer);
											GameController.Instance.Map.Markers.Remove(_markerB);
											_markers.Remove(_markerB);
											_markerB.Hide();
											_charactersWithOrbs.Add(parameters.Performer, _markerB);
											await GDTask.CompletedTask;
										},
										effectButtonParameters: new IconEffectButton.Parameters(Icons.GetMarker(Marker.Type.b)),
										effectInfoViewParameters: new TextEffectInfoView.Parameters(
											$"Take Orb {Icons.Inline(Icons.GetMarker(Marker.Type.b))}."),
										effectType: EffectType.Selectable
									),
									ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters =>
											GameController.Instance.Map.GetMarker(Marker.Type.c) != null &&
											RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Contains(_markerC.Hex),
										applyFunction: async applyParameters =>
										{
											SubscribeToMarkerC(parameters.Performer);
											GameController.Instance.Map.Markers.Remove(_markerC);
											_markers.Remove(_markerC);
											_markerC.Hide();
											_charactersWithOrbs.Add(parameters.Performer, _markerC);
											await GDTask.CompletedTask;
										},
										effectButtonParameters: new IconEffectButton.Parameters(Icons.GetMarker(Marker.Type.c)),
										effectInfoViewParameters: new TextEffectInfoView.Parameters(
											$"Take Orb {Icons.Inline(Icons.GetMarker(Marker.Type.c))}."),
										effectType: EffectType.Selectable
									),
									ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters =>
											GameController.Instance.Map.GetMarker(Marker.Type.d) != null &&
											RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Contains(_markerD.Hex),
										applyFunction: async applyParameters =>
										{
											SubscribeToMarkerD(parameters.Performer);
											GameController.Instance.Map.Markers.Remove(_markerD);
											_markers.Remove(_markerD);
											_markerD.Hide();
											_charactersWithOrbs.Add(parameters.Performer, _markerD);
											await GDTask.CompletedTask;
										},
										effectButtonParameters: new IconEffectButton.Parameters(Icons.GetMarker(Marker.Type.d)),
										effectInfoViewParameters: new TextEffectInfoView.Parameters(
											$"Take Orb {Icons.Inline(Icons.GetMarker(Marker.Type.d))}."),
										effectType: EffectType.Selectable
									),
								], hintText: "Choose an Orb to take");
							})
							.Build()
					]);
					await actionState.Perform();
				},
				EffectType.Selectable,
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"Take an Orb from an adjacent hex")
			);
		}

		if(parameters.OpenedDoor == _door2)
		{
			await ShowText(
				"""
				Grabbing a globe each, you immediately feel empowered by the glowing sphere. The energy force continues, and you can feel them urging you towards another roughly hewn door in the ice bound cavern. As you force your way through it and slam it behind you, you find peace from the storm, though there are more frozen horrors guarding the door, and the globes are pulling you towards the next door.
				""");
		}

		if(parameters.OpenedDoor == _door3)
		{
			AddScenarioRule(textParameters =>
				$"While occupying the K2b tile, all characters gain {Icons.Inline(Icons.GetCondition(Conditions.Chill), textParameters)} at the end of their turn.");

			_warmFiresRule.Remove();
			_warmFiresRule = AddScenarioRule(textParameters =>
				$"""
				 The Hot Coal hexes represents Warm Fires and cannot be removed. If a character ends their turn within {Icons.Inline(Icons.Range, textParameters)} 1 of a Warm Fire, they ignore the two effects above.
				 """);

			AddScenarioRule(textParameters =>
				$"""
				 The dome is represented by the altar. The altar cannot be destroyed. Each character may forgo the top or bottom action of their turn while adjacent to the dome to place the orb in the dome.
				 """);

			await ShowText(
				"""
				You open another door, and again the storm hits you. Although you are a little more prepared this time, the power is still breathtaking. The globes’ pull is even stronger now, almost dragging you towards a highly polished obsidian dome which somehow emanates a sense of stillness, out of place in this maelstrom of ice and wind. However, strong as the globes’ pull is, you are also beginning to feel a strange attachment to them and are reluctant to give them up, though you know deep down that the globes must be returned to the dome.
				""");

			//Gain Chill at end of round
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _door3,
				canApplyParameters =>
				{
					return canApplyParameters.Figure is Character;
				},
				async applyParameters =>
				{
					if(GameController.Instance.Map.Rooms[1].Hexes.Contains(applyParameters.Figure.Hex) &&
					   !RangeHelper.GetHexesInRange(applyParameters.Figure.Hex, 1).Any(hex => hex.HexObjects.Any(obj => _hotCoals.Contains(obj))))
					{
						await AbilityCmd.AddCondition(null, applyParameters.Figure, Conditions.Chill);
					}
				}
			);

			//Forgo action to place orb
			ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, _door3,
				parameters =>
					!parameters.ForgoneAction &&
					_charactersWithOrbs.ContainsKey(parameters.Performer) &&
					RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Contains(_dome.Hex),
				async parameters =>
				{
					parameters.ForgoAction();

					ActionState actionState = new ActionState(parameters.Performer, [
						OtherAbility.Builder()
							.WithPerformAbility(async state =>
							{
								Marker orbPlaced = _charactersWithOrbs[parameters.Performer];
								_charactersWithOrbs.Remove(parameters.Performer);

								await _goal.AdjustProgress(1);

								_orbsPlaced++;
								if(orbPlaced == _markerA)
								{
									ScenarioEvents.DuringMovementEvent.Unsubscribe(parameters.Performer, _markerA);
									ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, _markerA);
								}
								else if(orbPlaced == _markerB)
								{
									ScenarioEvents.DuringAttackEvent.Unsubscribe(parameters.Performer, _markerB);
									ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, _markerB);
								}
								else if(orbPlaced == _markerB)
								{
									ScenarioEvents.RetaliateEvent.Unsubscribe(parameters.Performer, _markerC);
									ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, _markerC);
								}
								else if(orbPlaced == _markerB)
								{
									ScenarioEvents.DuringAttackEvent.Unsubscribe(parameters.Performer, _markerD);
									ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(parameters.Performer, _markerD);
								}

								await GDTask.CompletedTask;
							})
							.Build()
					]);
					await actionState.Perform();
				},
				EffectType.Selectable,
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"Place an Orb in the dome.")
			);
		}
	}

	private void SubscribeToMarkerA(Figure figure)
	{
		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, _markerA,
			parameters => parameters.Performer == figure && parameters.AbilityState is MoveAbility.State,
			parameters =>
			{
				((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(1);
				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerA,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"This character adds +1{Icons.Inline(Icons.Move, textParameters)} to all move abilities."));
			}
		);
	}

	private void SubscribeToMarkerB(Figure figure)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(figure, _markerB, canApplyParameters => canApplyParameters.Performer == figure,
			async applyParameters =>
			{
				applyParameters.AbilityState.SingleTargetAdjustPierce(2);
				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerB,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"This character adds {Icons.Inline(Icons.Pierce, textParameters, true)} 2 to all attack abilities."));
			}
		);
	}

	private void SubscribeToMarkerC(Figure figure)
	{
		ScenarioEvents.RetaliateEvent.Subscribe(figure, _markerC, canApplyParameters => canApplyParameters.Performer == figure,
			async applyParameters =>
			{
				applyParameters.SetRetaliateBlocked();
				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerB,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
					$"This character is unaffected by {Icons.Inline(Icons.Retaliate, textParameters, true)}."));
			}
		);
	}

	private void SubscribeToMarkerD(Figure figure)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(figure, _markerD, canApplyParameters => canApplyParameters.Performer == figure,
			async applyParameters =>
			{
				applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Chill);
				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerD,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(
					new InfoTextExtraEffect.Parameters(textParameters =>
						$"This character adds {Icons.InlineCondition(Conditions.Chill, textParameters)} to all attack abilities."));
			}
		);
	}
}