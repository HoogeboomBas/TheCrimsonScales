using System.Linq;
using Fractural.Tasks;

public class Pickpocket : TheCrimsonScalesBattleGoal
{
	public override string Title => "Pickpocket";
	public override string Description => "Collect two or more loot tokens by performing a loot ability while adjacent to one or more enemies.";

	public override async GDTask OnScenarioSetupPhaseCompleted(Character character, BattleGoal battleGoal)
	{
		ScenarioEvents.AbilityPerformedEvent.Subscribe(character, this,
			parameters => 
				parameters.Performer == character &&
				parameters.AbilityState is LootAbility.State state &&
				character.Hex.Neighbours.Any(hex => hex.GetFigures().Any(figure => figure.EnemiesWith(character))),
			async parameters =>
			{
				battleGoal.AdjustProgress(1);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}
}