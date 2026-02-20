using System.Linq;

public class Granurso : CaveBear
{
	public override MonsterStats[] NamedLevelStats =>
	CharacterCount == 2
		? NormalLevelStats
		: EliteLevelStats;
	
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new AllNegativeConditionImmunityTrait())
				.Append(new ShieldTrait(stats.Attack / 2))
				.ToArray()
			})
			.ToArray();
	
	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new AllNegativeConditionImmunityTrait())
				.Append(new ShieldTrait(stats.Attack / 2))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Granurso";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<CaveBear>();
}