using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario024 : ScenarioModel
{
	//TODO: Currently all the button presses (except for the chill ones) don't have an icon since their is no file for a marker icon,
	//not sure where to find that/what else to put there
	public override string ScenePath => "res://Content/Scenarios/Scenario024.tscn";
	public override int ScenarioNumber => 24;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario026>(true)];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Place " + GameController.Instance.SavedCampaign.Characters.Count + " orbs in the dome to win this scenario." +
		                        System.Environment.NewLine + System.Environment.NewLine +
		                        "Any character may forgo the top or bottom action of their turn to remove all " +
		                        $"{Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one summon they own within {Icons.Inline(Icons.Range)} 2.");

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private Door _door1;
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

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<CursebloodBlade>());

		_markerA = GameController.Instance.Map.GetMarker(Marker.Type.a);

		_markerB = GameController.Instance.Map.GetMarker(Marker.Type.b);

		_markerC = GameController.Instance.Map.GetMarker(Marker.Type.c);

		_markerD = GameController.Instance.Map.GetMarker(Marker.Type.d);

		_markers.AddRange([_markerA, _markerB, _markerC, _markerD]);

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
		_door3 = marker3.GetHexObject<Door>();

		_hotCoals = GameController.Instance.Map.GetChildrenOfType<HazardousTerrain>();

		foreach(HazardousTerrain hotCoal in _hotCoals)
		{
			hotCoal.SetCannotBeDestroyed(true);
		}

		List<Obstacle> obstacles = GameController.Instance.Map.GetChildrenOfType<Obstacle>();
		_dome = obstacles[obstacles.Count() - 1];
		_dome.SetCannotBeDestroyed(true);

		//Scenario Win Condition
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters =>
			{
				return _orbsPlaced == GameController.Instance.SavedCampaign.Characters.Count;
			},
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			}
		);

		//Remove Chill forgo action
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
				.Where(figure => figure.HasCondition(Conditions.Chill) &&
				                 ((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)).Any(),
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

							await AbilityCmd.RemoveAllChill(figure);
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
			UpdateScenarioText($"""
			                    While adjacent to the hexes marked {Icons.InlineMarker(Marker.Type.a)}, {Icons.InlineMarker(Marker.Type.b)}, {Icons.InlineMarker(Marker.Type.c)}, {Icons.InlineMarker(Marker.Type.d)}, each character may forgo the top or bottom action of their turn to pick up the letter representing each Orb and gain the following bonus:
			                    {Icons.InlineMarker(Marker.Type.a)}: Add +1{Icons.Inline(Icons.Move)} to all your move abilities
			                    {Icons.InlineMarker(Marker.Type.b)}: Add {Icons.Inline(Icons.Pierce)} 2 to all your attack abilities
			                    {Icons.InlineMarker(Marker.Type.c)}: You are unaffected by {Icons.Inline(Icons.Retaliate)}
			                    {Icons.InlineMarker(Marker.Type.d)}: Add {Icons.Inline(Icons.GetCondition(Conditions.Chill))} to all your attack abilities
			                    Each character may only hold a maximum of one orb. If any character exhausts while holding an orb, the scenario is lost.
			                    While occupying the K1b tile, at the end of each character and character summons turn, if they have no {Icons.Inline(Icons.GetCondition(Conditions.Chill))} tokens they gain {Icons.Inline(Icons.GetCondition(Conditions.Chill))}.
			                    The Hot Coal hexes represents Warm Fires and cannot be removed. If a character ends their turn within {Icons.Inline(Icons.Range)} 1 of a Warm Fire, they ignore this effect.
			                    """);

			//lose if character exhausts with an orb
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				canApplyParamaters => canApplyParamaters.Figure is Character character && _charactersWithOrbs.ContainsKey(character),
				async parameters =>
				{
					await AbilityCmd.Lose();
				}
			);

			//Gain chill at end of round 
			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _door1,
				canApplyParameters =>
				{
					return canApplyParameters.Figure is Character || canApplyParameters.Figure is Summon;
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

			//forgo action to take an orb
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
										effectInfoViewParameters: new TextEffectInfoView.Parameters($"Take Orb A"),
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
										effectInfoViewParameters: new TextEffectInfoView.Parameters($"Take Orb B"),
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
										effectInfoViewParameters: new TextEffectInfoView.Parameters($"Take Orb C"),
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
										effectButtonParameters: new IconEffectButton.Parameters(null),
										effectInfoViewParameters: new TextEffectInfoView.Parameters($"Take Orb D"),
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

		if(parameters.OpenedDoor == _door3)
		{
			UpdateScenarioText($"""
			                    While occupying the K2b tile, all characters gain {Icons.Inline(Icons.GetCondition(Conditions.Chill))} at the end of their turn

			                    The Hot Coal hexes represents Warm Fires and cannot be removed. If a character ends their turn within {Icons.Inline(Icons.Range)} 1 of a Warm Fire, they ignore this effect
			                    The dome is represented by the altar. The altar cannot be destroyed. Each character may forgo the top or bottom action of their turn while adjacent to the dome to place the orb in the dome.
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

			//forgo action to place orb
			ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, _door3,
				parameters => !parameters.ForgoneAction && _charactersWithOrbs.ContainsKey(parameters.Performer) &&
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
				parameters.Add(new FigureInfoTextExtraEffect.Parameters($"Add +1{Icons.Inline(Icons.Move)} to all move abilities"));
			}
		);
	}

	private void SubscribeToMarkerB(Figure figure)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(figure, _markerB, canApplyParameters => canApplyParameters.Performer == figure,
			async applyParameters =>
			{
				applyParameters.AbilityState.AbilityAdjustPierce(2);
				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerB,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(new FigureInfoTextExtraEffect.Parameters($"Add {Icons.Inline(Icons.Pierce)} 2 to all attack abilities"));
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
				parameters.Add(new FigureInfoTextExtraEffect.Parameters($"Unaffected by {Icons.Inline(Icons.Retaliate)}"));
			}
		);
	}

	private void SubscribeToMarkerD(Figure figure)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(figure, _markerD, canApplyParameters => canApplyParameters.Performer == figure,
			async applyParameters =>
			{
				applyParameters.AbilityState.AbilityAddCondition(Conditions.Chill);
				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(figure, _markerD,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.Add(
					new FigureInfoTextExtraEffect.Parameters($"Add {Icons.Inline(Icons.GetCondition(Conditions.Chill))} to all attack abilities"));
			}
		);
	}
}