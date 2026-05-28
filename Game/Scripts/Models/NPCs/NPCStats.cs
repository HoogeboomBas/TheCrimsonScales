public class NPCStats
{
	public int Health { get; init; } = 0;
	public int? Move { get; init; } = null;
	public int? Attack { get; init; } = null;
	public int? Range { get; init; } = null;

	public FigureTrait[] Traits { get; init; } = null;

	public RangeType RangeType => Range.HasValue ? RangeType.Range : RangeType.Melee;
}