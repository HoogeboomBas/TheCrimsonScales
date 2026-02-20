using System.Linq;

public class GoringGrizzly : CaveBear
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(ConditionImmunityTrait.PoisonImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Goring Grizzly";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<CaveBear>();
}