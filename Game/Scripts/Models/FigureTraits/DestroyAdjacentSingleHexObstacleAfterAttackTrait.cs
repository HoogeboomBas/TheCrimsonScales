using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DestroyAdjacentSingleHexObstacleAfterAttackTrait() : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(figure, this,
			parameters => parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				List<Hex> adjacentHexList = new();
				RangeHelper.FindHexesInRange(parameters.AbilityState.Target.Hex, 1, true, adjacentHexList);

				// Select hexes that have a 1-hex obstacle
				Hex selectedHex =
					await AbilityCmd.SelectHex(parameters.AbilityState,
						list => list.AddRange(adjacentHexList
							.Where(hex => hex.GetHexObjectsOfType<Obstacle>()
								.Any(obstacle => obstacle.Hexes.Length == 1))),
						false, "Select a 1-hex obstacle to destroy");

				if(selectedHex != null)
				{
					await AbilityCmd.TryDestroyObstacle(selectedHex.GetHexObjectsOfType<Obstacle>()
						.FirstOrDefault(obstacle => obstacle.Hexes.Length == 1));
				}
			}
		);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(figure, this);
	}
}