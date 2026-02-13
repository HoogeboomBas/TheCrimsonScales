public class AIMoveParameters
{
	public int Targets { get; set; } = 1;
	public bool TargetAll { get; set; } = false;

	public int Range { get; set; } = 1;
	public RangeType? RangeType { get; set; } = null;
	public AOEPattern AOEPattern { get; set; } = null;
	public MoveType MoveType { get; set; } = MoveType.Regular;
}