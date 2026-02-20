using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario019 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario019.tscn";
	public override int ScenarioNumber => 19;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<WondrousScenarioChain>();

	public override IEnumerable<ScenarioConnection> Connections =>
	[
		new ScenarioConnection<Scenario023>(), new ScenarioConnection<Scenario024>(), /*new ScenarioConnection<Scenario025>()*/
	];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	private IEnumerable<PressurePlate> _pressurePlates;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		_pressurePlates = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<PressurePlate>());

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<InfraredGoggles>());
		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async character =>
		{
			await new ActionState(character, [HealAbility.Builder().WithHealValue(6).WithTarget(Target.Self).Build()]).Perform();
			await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
		});

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => parameters.Figure is Character &&
			              _pressurePlates.Select(pressurePlate => pressurePlate.Hex).Contains(parameters.Figure.Hex),
			async parameters =>
			{
				for(int i = 0;
				    i < parameters.Figure.Hex.GetRoom().Figures
					    .Count(figure => figure is Monster monster && monster.MonsterModel is VermlingExperiment);
				    i++)
				{
					if(await AbilityCmd.AskConsumeWildElement(parameters.Figure) == null)
					{
						break;
					}
				}
			});

		//TODO: Change to any element symbol
		UpdateScenarioText($"""
		                    The Vermling Scouts represent Vermling Experiments, and gain benefits based on the number of strong or waning elements.
		                    Vermling Experiments gain {Icons.Inline(Icons.Shield)}X and {Icons.Inline(Icons.Retaliate)}X, where X is the number of strong and waning elements divided by two (rounded up).

		                    If any character ends their turn on a pressure plate, they may consume any element X times, where X is the number of Vermling Experiments in the room.
		                    """);
	}
}