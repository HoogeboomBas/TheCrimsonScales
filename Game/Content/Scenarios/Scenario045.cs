using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario045 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario045.tscn";
	public override int ScenarioNumber => 45;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new KillSpecificEnemiesTypeGoals(
		ModelDB.Monster<LandLeviathan>(), "Kill the Land Leviathan to win this scenario.");

	private IEnumerable<Hex> _markerAHexes;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_markerAHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		UpdateScenarioText($"""
		                    The Deep Terror represents the Land Leviathan. The Land Leviathan draws from the boss ability deck and performs the following specials:

		                    Special 1: {Icons.Inline(Icons.Attack)}-1, {Icons.Inline(Icons.Range)}5, {Icons.Inline(Icons.Targets)}2. Increase the Land Leviathan's maximum hit point value by 2. {Icons.Inline(Icons.Heal)} 2, Self.

		                    Special 2: Summon one Imp in the closest empty hex within {Icons.Inline(Icons.Range)}2. Grant all Imps within {Icons.Inline(Icons.Range)}5 perform “Heal 1, Self.” The type of Imp that is summoned cycles in the order of Black Imp, then Forest Imp. All summons are normal for two characters. Black Imp summons are elite for three characters. All summons are elite for four characters. All summons are elite for four characters.

		                    Spawn one of the following monsters groups in the hexes marked {Icons.Inline(Icons.GetMarker(Marker.Type.a))}. These monsters are allies to you and each other and enemies to all other monsters, are immune to {Icons.Inline(Icons.GetCondition(Conditions.Curse))} and draw from the monster attack modifier deck. Choose one of the following groups:

		                    Two normal Cave Bears
		                    One normal Rending Drake and one normal Spitting Drake
		                    Two normal Lurkers and one elite Giant Viper
		                    """);

		await AbilityCmd.GenericChoice(GameController.Instance.CharacterManager.FirstAlive(),
		[
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnMonster(ModelDB.Monster<CaveBear>(), MonsterType.Normal);
					await SpawnMonster(ModelDB.Monster<CaveBear>(), MonsterType.Normal);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && monster.MonsterModel is CaveBear))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/CaveBear/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn two normal Cave Bears"),
				effectType: EffectType.SelectableMandatory
			),
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnMonster(ModelDB.Monster<RendingDrake>(), MonsterType.Normal);
					await SpawnMonster(ModelDB.Monster<SpittingDrake>(), MonsterType.Normal);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && (monster.MonsterModel is RendingDrake || monster.MonsterModel is SpittingDrake)))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/RendingDrake/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn one normal Rending Drake and one normal Spitting Drake"),
				effectType: EffectType.SelectableMandatory
			),
			ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
				applyFunction: async applyParameters =>
				{
					await SpawnMonster(ModelDB.Monster<Lurker>(), MonsterType.Normal);
					await SpawnMonster(ModelDB.Monster<Lurker>(), MonsterType.Normal);
					await SpawnMonster(ModelDB.Monster<GiantViper>(), MonsterType.Elite);
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
						        figure is Monster monster && (monster.MonsterModel is GiantViper || monster.MonsterModel is Lurker)))
					{
						ConditionImmunityTrait curseImmunity = new ConditionImmunityTrait(Conditions.Curse);
						await curseImmunity.Activate(figure);
						ScenarioEvents.FigureKilledEvent.Subscribe(this, figure,
							parameters => parameters.Figure == figure,
							async parameters =>
							{
								await curseImmunity.Deactivate(figure);
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this, figure);
							});
					}
				},
				effectButtonParameters: new IconEffectButton.Parameters("res://Content/Monsters/Lurker/MapIcon.tres"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters("Spawn two normal Lurkers and one elite Giant Viper"),
				effectType: EffectType.SelectableMandatory
			),
		], hintText: "Choose a group of monsters to spawn");
	}

	private async GDTask SpawnMonster(MonsterModel monsterModel, MonsterType monsterType)
	{
		await SpawnMonster(null, monsterModel, monsterType, _markerAHexes, GameController.Instance.SavedScenario.ScenarioLevel - 1,
			Alignment.Characters, Alignment.Enemies);
	}
}