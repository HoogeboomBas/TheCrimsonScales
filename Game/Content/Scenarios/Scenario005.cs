using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario005 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario005.tscn";
	public override int ScenarioNumber => 5;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario006>()];

	//protected override ScenarioGoals CreateScenarioGoals() => new KillSpecificEnemyTypeGoals(ModelDB.Monster<GelatinousGiant>(), "Kill the Gelatinous Giant to win this scenario.");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText(
			$"All characters start with INFECT {Icons.Inline(Icons.GetCondition(Conditions.Infect))} as a scenario effect");

		IEnumerable<Figure> characterFigures = GameController.Instance.Map.Figures.Where(figure => figure is Character);
		foreach(Figure figure in characterFigures)
		{
			await AbilityCmd.AddCondition(null, figure, Conditions.Infect);
		}
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		UpdateScenarioText(
			$"The door is locked. When a character ends their turn on the pressure plate marked {Icons.Marker(Marker.Type.b)} the door is permanently unlocked " +
			$"and all figures occupying the H1A map tile perform a “{Icons.Inline(Icons.Heal)} 2, Self” ability.");

		Figure boss = GameController.Instance.Map.Figures.Where(figure => figure is Monster monsterFigure && monsterFigure.MonsterModel is GelatinousGiant).First();
		
	}
}

