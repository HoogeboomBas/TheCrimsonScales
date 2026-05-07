using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class LootGoalTreasuresGoal : ScenarioGoal
{
	private readonly List<Treasure> _treasures;

	public LootGoalTreasuresGoal(int order = 0)
		: base(order)
	{
		_treasures = GameController.Instance.Map.Treasures.Where(treasure => treasure.IsGoal).ToList();
	}

	public LootGoalTreasuresGoal(List<Treasure> treasures, int order = 0)
		: base(order)
	{
		_treasures = treasures.ToList();
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		_treasures.Count == 1 ? "Loot the Goal treasure tile." : "Loot all Goal treasure tiles.";

	public override async GDTask Start()
	{
		await base.Start();

		ScenarioEvents.LootableObjectLootedEvent.Subscribe(this,
			parameters => parameters.LootableObject is Treasure treasure && treasure.IsGoal,
			async parameters =>
			{
				await AdjustProgress(1);
			}
		);

		await SetMaxProgress(_treasures.Count);
	}
}