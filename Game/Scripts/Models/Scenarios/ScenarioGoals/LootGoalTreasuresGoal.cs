using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class LootGoalTreasuresGoal : ScenarioGoal
{
	private readonly List<Treasure> _treasures;
	private readonly int? _requiredTreasureCount;

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

	public LootGoalTreasuresGoal(int? requiredTreasureCount, int order = 0)
		: base(order)
	{
		_treasures = GameController.Instance.Map.Treasures.Where(treasure => treasure.IsGoal).ToList();
		_requiredTreasureCount = requiredTreasureCount;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		_requiredTreasureCount.HasValue
			? _requiredTreasureCount == 1
				? $"Loot a treasure tile."
				: $"Loot at least {_requiredTreasureCount.Value} treasure tiles."
			: _treasures.Count == 1
				? "Loot the Goal treasure tile."
				: "Loot all Goal treasure tiles.";

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

		await SetMaxProgress(_requiredTreasureCount ?? _treasures.Count);
	}
}