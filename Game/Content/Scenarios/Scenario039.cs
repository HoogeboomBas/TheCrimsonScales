using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario039 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario039.tscn";
	public override int ScenarioNumber => 39;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals(
			$"All characters must occupy a hex {Icons.InlineMarker(Marker.Type.a)} or become exhausted on a hex {Icons.InlineMarker(Marker.Type.a)} to win this scenario.");
	protected override List<MonsterModel> SpawnedMonsterModels { get; } =
		[ModelDB.Monster<EarthDemon>()];

	private List<Hex> _markerAHexes;
	private Hex _markerBHex;
	private Hex _markerCHex;
	private string _text;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemDesignLoot(ModelDB.Item<VipertoothDagger>());

		//TODO: Scenario Effect

		_markerAHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex).ToList();
		_markerBHex = GameController.Instance.Map.GetMarker(Marker.Type.b).Hex;
		_markerCHex = GameController.Instance.Map.GetMarker(Marker.Type.c).Hex;

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters => parameters.Performer is Monster monster && monster.MonsterModel is GhostViperScenario039 &&
			              parameters.AbilityState.Target.HasPoison(),
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);
				await GDTask.CompletedTask;
			});

		ScenarioEvents.InflictConditionEvent.Subscribe(this,
			parameters => parameters.PotentialAbilityState?.Performer is Monster monster && monster.MonsterModel is GhostViperScenario039 &&
			              parameters.ConditionModel == Conditions.Immobilize,
			async parameters =>
			{
				parameters.SetPrevented(true);
				await AbilityCmd.AddCondition(parameters.PotentialAbilityState, parameters.Target, Conditions.Muddle);
			});

		//Win and Lose Conditions
		ScenarioEvents.RoundEndedEvent.Subscribe(this, ScenarioGoals,
			parameters => GameController.Instance.Map.Figures.Where(figure => figure is Character)
				.All(character => _markerAHexes.Contains(character.Hex)),
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			});

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Character && !_markerAHexes.Contains(parameters.Figure.Hex),
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Lose();
			}
		);

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			parameters => parameters.Figure is Character character && character.SavedCharacter?.SavedPersonalQuest.Model is AnAdderDivides &&
			              GameController.Instance.ScenarioPhaseManager.RoundIndex + 1 < 9,
			parameters =>
			{
				parameters.SetCanEnter(false);
			});

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters => parameters.Figure is Character character && character.SavedCharacter?.SavedPersonalQuest.Model is AnAdderDivides,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters("Viper Hunter (see special rules)"));
			});

		_text += $"""
		          The character with An Adder Divides Personal quest is the Viper Hunter. They cannot leave their starting hex in any way until the start of the ninth round. This is not considered a scenario effect.

		          If any character becomes exhausted while they are not occupying a hex {Icons.InlineMarker(Marker.Type.a)}, the scenario is lost.

		          The Giant Vipers are Ghost Vipers and they add +1{Icons.Inline(Icons.Attack)} to each attack targeting a figure that has {Icons.Inline(Icons.GetCondition(Conditions.Poison1))}.
		          Whenever a Ghost Viper would give {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} to a figure, that figure does not gain {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))} and gains {Icons.Inline(Icons.GetCondition(Conditions.Muddle))} instead.

		          At the start of each round, draw an additional ability card for the Ghost Vipers. Even numbered Ghost Vipers act according to the first drawn ability card and odd numbered Ghost Vipers act according to the second drawn ability card.
		          """;
		UpdateScenarioText(_text);

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
					});
				_text += $"""


				          Any character may forgo a top or bottom action to remove {Icons.Inline(Icons.GetCondition(Conditions.Poison1))} from themselves or one adjacent ally.

				          At the start of each round Ghost Vipers spawn on the nearest unoccupied hexes to hex {Icons.InlineMarker(Marker.Type.b)}. Spawn one normal and one elite Ghost Viper for two characters, two normal and one elite Ghost Viper for three characters, and spawn three elite Ghost Vipers for four characters.
				          Any time a Ghost Viper would be spawned when there are not enough standees to do so, each character and character summon suffers {Icons.Inline(Icons.Damage)}2.
				          """;
				UpdateScenarioText(_text);

				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

				ScenarioEvents.RoundEndedEvent.Subscribe(this,
					parameters => roundEndedParameters.RoundNumber == 6,
					async parameters =>
					{
						_text += $"""


						          Remove {GameController.Instance.SavedCampaign.Characters.Count} boulder obstacles, and spawn a normal Earth Demon in each of the hexes from which they were removed. If there are not enough boulder obstacles present on the map at this point, each Earth Demon that cannot be spawned in these hexes is instead spawned on hex these hexes is instead spawned on hex {Icons.InlineMarker(Marker.Type.c)} and is an elite enemy. If any figure occupies one of the hexes in which a demon should spawn, that figure suffers {Icons.Inline(Icons.Damage)}3 and the demon is spawned in the nearest unoccupied hex instead.
						          """;
						UpdateScenarioText(_text);
						for(int i = 0; i < GameController.Instance.SavedCampaign.Characters.Count; i++)
						{
							Hex hex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(), list =>
									list.AddRange(GameController.Instance.Map.GetChildrenOfType<Boulder1HObstacle>()
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
					});
				await GDTask.CompletedTask;
			});

		//TODO: Have half the ghost vipers draw a different card
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