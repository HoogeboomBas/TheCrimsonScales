using System.Linq;

public class GhostViperScenario047 : GiantViper
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Traits = [new ApplyConditionTrait(Conditions.Poison2)]
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = [new ApplyConditionTrait(Conditions.Poison2)]
			})
			.ToArray();

	public override string Name => "Ghost Viper";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<GiantViper>();
}