using System.Linq;

public class HangingSnake : GiantViper
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * 2,
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * 2,
			})
			.ToArray();

	public override string Name => "Hanging Snake";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<GiantViper>();
}