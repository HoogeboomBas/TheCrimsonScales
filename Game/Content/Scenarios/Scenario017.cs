using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario017 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario017.tscn";
	public override int ScenarioNumber => 17;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario018>(true)];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals(
			$"Loot the Goal treasure tile and have all characters occupy hexes with {Icons.InlineMarker(Marker.Type.a)} to win this scenario.");

	private bool _treasureLooted;
	private IEnumerable<Hex> _markerHexes;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_markerHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 6).SetObtainLootFunction(async character =>
		{
			character.SavedCharacter.AddGold(20);
			foreach(Trap trap in RangeHelper.GetHexesInRange(character.Hex, 1).SelectMany(hex => hex.GetHexObjectsOfType<Trap>())
				        .Where(trap => trap != null))
			{
				await AbilityCmd.DisarmTrap(trap, character);
			}
		});
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 19).SetObtainLootFunction(async character =>
		{
			character.SavedCharacter.AddCheckmark();
			await AbilityCmd.InfuseWildElement(null, character);
		});
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 43).SetObtainLootFunction(async character =>
		{
			character.SavedCharacter.AddXP(10);
			foreach(ItemModel item in character.Items.Where(item => item.ItemState == ItemState.Spent))
			{
				await AbilityCmd.RefreshItem(item);
			}
		});
		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == -1).SetObtainLootFunction(async character =>
		{
			_treasureLooted = true;
			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
				parameters => parameters.Performer == character && parameters.AbilityState is AttackAbility.State or MoveAbility.State,
				async parameters =>
				{
					switch(parameters.AbilityState)
					{
						case MoveAbility.State moveAbilityState:
							moveAbilityState.AdjustMoveValue(1);
							break;
						case AttackAbility.State attackAbilityState:
							attackAbilityState.AbilityAdjustAttackValue(1);
							break;
					}

					await GDTask.CompletedTask;
				});
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async parameters =>
				{
					await ((CustomScenarioGoals)ScenarioGoals).Lose();
				});
			await GDTask.CompletedTask;
		});

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => _treasureLooted &&
			              GameController.Instance.CharacterManager.Characters.All(character => _markerHexes.Contains(character.Hex)),
			async parameters =>
			{
				await ((CustomScenarioGoals)ScenarioGoals).Win();
			});

		UpdateScenarioText($"""
		                    At the start of the scenario, nominate one character to carry the Frosted Crystal. This character may not loot the goal tile and gains {Icons.Inline(Icons.Retaliate)}1.

		                    The exit is indicated by the starting hexes. The goal treasure tile represents the Orb of Embers. While a character possesses the Orb of Embers, the character adds +1 to all attack abilities and move abilities. If the character who holds the Orb of Embers becomes exhausted, the scenario is immediately lost.
		                    """);

		Figure frostedCrystalCharacter = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to gain the Frosted Crystal");

		ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.Figure == frostedCrystalCharacter,
			applyParameters =>
			{
				applyParameters.AddRetaliate(1, 1);
			});

		ScenarioEvents.RetaliateEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.RetaliatingFigure == frostedCrystalCharacter &&
			                      RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, frostedCrystalCharacter.Hex) <= 1,
			async applyParameters =>
			{
				applyParameters.AdjustRetaliate(1);

				await GDTask.CompletedTask;
			});

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == -1).CanLootFunction =
			figure => figure != frostedCrystalCharacter;
	}
}