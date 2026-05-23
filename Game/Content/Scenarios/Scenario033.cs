using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario033 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario033.tscn";

	public override int ScenarioNumber => 33;
	public override string Name => "Siege Tower";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();

	public override string IntroductionText =>
		"""
		You’ve spent the morning sleeping off the excess of ale from the night before. Your head is booming, It sounds like thunder. Boom! Boom! Wait... there is a booming! It’s not all in your head, it’s coming from outside. You hurry outside of your hovel and into the streets. Guards are scrambling this way and that. “Inox!” one of them calls. “At the Western Gate! Positions!”

		There is a mad scramble of guardsmen emerging from the armory with weapons in hand. A strange little Quatryl readies various black powders and struggles to heave cannon balls onto a pile near the artillery. In your distracted stare you collide into one of the running guards. “Don’t just stand there, mercenary! Aid your city! We need all the fighting power we can get!”

		It’s unusual but not unheard of that Inox raiding parties would attack the city. Growing logging operations have angered the local tribes into retribution. It’s not like you to get involved in the local politics here but you’d rather not have your city burned to the ground. Arrows whistle over-top the wall and the sounds of steel clashing on steel can be heard just on the other side. Your head still throbs with pain but it’s clear the Inox will do much worse if you do not defend the walls and aid the overpowered guard.
		""";

	public override string ConclusionText =>
		"""
		The carnage of the raid lays before you as you make your way off the battlefield towards the gate. The wounded Inox lays bleeding on the pavestones. “We’re not... done... here... city-dweller” He cough up blood and continues “Orgrum Bonebreaker will... not... stop until... we’ve gotten our... vengeance!” He lets out his final breath. Ahead you see the city guard cheering the recent victory. You should probably let them know the celebration is a bit premature.
		""";

	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario034>(true)];

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxBodyguard>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<InoxShaman>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario034>())
	];

	private IEnumerable<Hex> _hexesLeftOfStaircase;
	private PressurePlate _markerAPressurePlate;
	private Hex _markerAHex;
	private PressurePlate _markerBPressurePlate;
	private Hex _markerBHex;
	private PressurePlate _markerCPressurePlate;
	private Hex _markerCHex;
	private Hex _markerDHex;
	private Hex _markerEHex;
	private PressurePlate _lastActivated;

	private KillAllEnemiesScenarioGoal _goal;
	private ScenarioRule _inoxDeadRule;
	private ScenarioRule _allEnemiesDeadRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new KillAllEnemiesScenarioGoal(true));

		AddScenarioRule("Only one character may occupy the walled area to the left of the staircase hex at any time.");
		AddScenarioRule(textParameters =>
			$"If a character ends its turn on a pressure plate, all figures within {Icons.Inline(Icons.Range, textParameters)}1 of the corresponding letter on the board immediately suffer {Icons.Inline(Icons.Damage, textParameters)}{(GameController.Instance.SavedScenario.ScenarioLevel + 1) / 2 + 1}. If at least one enemy suffers damage this way, the character gains 1{Icons.Inline(Icons.XP, textParameters)}.");
		AddScenarioRule("The same pressure plate cannot be activated twice in a row.");

		_hexesLeftOfStaircase = GameController.Instance.Map.GetMarkers(Marker.Type._1).Select(marker => marker.Hex);
		_markerAPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.a)[0].GetHexObject<PressurePlate>();
		_markerBPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.b)[0].GetHexObject<PressurePlate>();
		_markerCPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.c)[0].GetHexObject<PressurePlate>();
		_markerAHex = GameController.Instance.Map.GetMarkers(Marker.Type.a)[1].Hex;
		_markerBHex = GameController.Instance.Map.GetMarkers(Marker.Type.b)[1].Hex;
		_markerCHex = GameController.Instance.Map.GetMarkers(Marker.Type.c)[1].Hex;
		_markerDHex = GameController.Instance.Map.GetMarkers(Marker.Type.d)[0].Hex;
		_markerEHex = GameController.Instance.Map.GetMarkers(Marker.Type.e)[0].Hex;

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character && _hexesLeftOfStaircase.Contains(parameters.Hex) &&
				_hexesLeftOfStaircase.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
					.Any(figure => figure is Character && figure != parameters.Figure),
			parameters =>
			{
				parameters.SetCanEnter(false);
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerAHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerAPressurePlate.Hex &&
			              _lastActivated != _markerAPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerAHex, _markerAPressurePlate);
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerBHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerBPressurePlate.Hex &&
			              _lastActivated != _markerBPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerBHex, _markerBPressurePlate);
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerCHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerCPressurePlate.Hex &&
			              _lastActivated != _markerCPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerCHex, _markerCPressurePlate);
			}
		);

		_inoxDeadRule = AddScenarioRule("Something will happen when all Inox are dead.");

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(false) == 0,
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

				_inoxDeadRule.Remove();

				await ShowText(
					"With the initial wave of Inox defeated, you lift your head to assess the battlefield. It’s clear that this battle is not over yet. One large, very brutish Inox emerges from their front line. Covered in scars and battle wounds, he bangs his fist on his chest. His massive hands move to his mouth and with a shrill whistle vicious hounds charge into the open battlefield.");

				_allEnemiesDeadRule = AddScenarioRule("Something will happen when all enemies are dead");

				await SpawnMonster(null, ModelDB.Monster<Hound>(), MonsterType.Elite, _markerEHex);
				await SpawnMonster(null, ModelDB.Monster<Hound>(), MonsterType.Elite, _markerEHex);
				int characterCount = GameController.Instance.SavedCampaign.Characters.Count;
				switch(characterCount)
				{
					case 2:
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Normal, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Normal, _markerDHex);
						break;
					case 3:
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Normal, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Normal, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), MonsterType.Elite, _markerAHex);
						break;
					case 4:
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Normal, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Elite, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxArcher>(), MonsterType.Elite, _markerDHex);
						await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), MonsterType.Elite, _markerAHex);
						break;
				}

				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					parameters =>
						KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(false) == 0,
					async parameters =>
					{
						ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

						_allEnemiesDeadRule.Remove();

						await ShowText(
							"You wipe the blood off of your blade with the hounds dispatched around you. Your head still throbs with pain but now it’s mixed with the strain of battle. Shouts from above on the wall continue to send orders to the artillery directing their fire. Nursing a wound at your side, you look up to see a very large Inox warrior approaching hefting a double-bladed axe.");

						ScenarioRule tempRule = AddScenarioRule(textParameters =>
							$"Each character immediately performs {Icons.Inline(Icons.Heal, textParameters)}4, Self.");

						foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure => figure is Character))
						{
							await new ActionState(figure, [HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()]).Perform();
						}

						tempRule.Remove();

						await SpawnMonster(null, ModelDB.Monster<InoxBodyguard>(), MonsterType.Boss, _markerDHex);
						if(characterCount is 3 or 4)
						{
							await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), characterCount == 3 ? MonsterType.Normal : MonsterType.Elite,
								_markerDHex);
						}

						await _goal.DisableEnemiesToBeSpawned();
					}
				);
			}
		);
	}

	private async GDTask PressurePlateActivated(Figure character, Hex hex, PressurePlate pressurePlate)
	{
		List<Figure> figures = RangeHelper.GetFiguresInRange(hex, 1).ToList();
		foreach(Figure figure in figures)
		{
			await AbilityCmd.SufferDamage(figure, (GameController.Instance.SavedScenario.ScenarioLevel + 1) / 2 + 1, character);
		}

		if(figures.Any(figure => figure.EnemiesWith(character)))
		{
			await AbilityCmd.GainXP(character, 1);
		}

		ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Unsubscribe(this);

		ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters => parameters.HexObject == pressurePlate,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This pressure plate cannot be activated until another one is."));
			}
		);
		_lastActivated = pressurePlate;
	}
}