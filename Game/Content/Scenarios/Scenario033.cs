using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario033 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario033.tscn";
	public override int ScenarioNumber => 33;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<PersonalQuestScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario034>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals(true);

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


	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_hexesLeftOfStaircase = GameController.Instance.Map.GetMarkers(Marker.Type._1).Select(marker => marker.Hex);
		_markerAPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.a)[0].GetHexObject<PressurePlate>();
		_markerBPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.b)[0].GetHexObject<PressurePlate>();
		_markerCPressurePlate = GameController.Instance.Map.GetMarkers(Marker.Type.c)[0].GetHexObject<PressurePlate>();
		_markerAHex = GameController.Instance.Map.GetMarkers(Marker.Type.a)[1].Hex;
		_markerBHex = GameController.Instance.Map.GetMarkers(Marker.Type.b)[1].Hex;
		_markerCHex = GameController.Instance.Map.GetMarkers(Marker.Type.c)[1].Hex;
		_markerDHex = GameController.Instance.Map.GetMarkers(Marker.Type.d)[0].Hex;
		_markerEHex = GameController.Instance.Map.GetMarkers(Marker.Type.e)[0].Hex;

		GD.Print(_markerAPressurePlate);

		ScenarioCheckEvents.CanEnterCheckEvent.Subscribe(this,
			parameters => parameters.Figure is Character && _hexesLeftOfStaircase.Contains(parameters.Hex) &&
			              _hexesLeftOfStaircase.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
				              .Any(figure => figure is Character && figure != parameters.Figure),
			parameters =>
			{
				parameters.SetCanEnter(false);
			});

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerAHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerAPressurePlate.Hex &&
			              _lastActivated != _markerAPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerAHex, _markerAPressurePlate);
			});
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerBHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerBPressurePlate.Hex &&
			              _lastActivated != _markerBPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerBHex, _markerBPressurePlate);
			});
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _markerCHex,
			parameters => parameters.Figure is Character && parameters.Figure.Hex == _markerCPressurePlate.Hex &&
			              _lastActivated != _markerCPressurePlate,
			async parameters =>
			{
				await PressurePlateActivated(parameters.Figure, _markerCHex, _markerCPressurePlate);
			});

		UpdateScenarioText("Something will happen when all Inox are dead");

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => KillAllEnemiesScenarioGoals.NoEnemiesRemaining(),
			async parameters =>
			{
				UpdateScenarioText("Something will happen when all enemies are dead");
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

				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					parameters => KillAllEnemiesScenarioGoals.NoEnemiesRemaining(),
					async parameters =>
					{
						UpdateScenarioText($"Each character immediately performs {Icons.Inline(Icons.Heal)}4, Self.");
						foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure => figure is Character))
						{
							await new ActionState(figure, [HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()]).Perform();
						}

						await SpawnMonster(null, ModelDB.Monster<InoxBodyguard>(), MonsterType.Boss, _markerDHex);
						if(characterCount is 3 or 4)
						{
							await SpawnMonster(null, ModelDB.Monster<InoxShaman>(), characterCount == 3 ? MonsterType.Normal : MonsterType.Elite,
								_markerDHex);
						}

						((KillAllEnemiesScenarioGoals)ScenarioGoals).EnemiesToBeSpawned = false;
					});
			});
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
				parameters.Add(new InfoTextExtraEffect.Parameters("This pressure plate cannot be activated until another one is."));
			}
		);
		_lastActivated = pressurePlate;
	}

	protected override void UpdateScenarioText(string text)
	{
		base.UpdateScenarioText(text + $"""


		                                Only one character may occupy the walled area to the left of the staircase hex at any time.

		                                If a character ends its turn on a pressure plate, all figures within {Icons.Inline(Icons.Range)}1 of the corresponding letter on the board immediately suffer {Icons.Inline(Icons.Damage)}{(GameController.Instance.SavedScenario.ScenarioLevel + 1) / 2 + 1}. If at least one enemy suffers damage this way, the character gains 1 {Icons.Inline(Icons.XP)}.

		                                The same pressure plate cannot be activated twice in a row.
		                                """);
	}
}