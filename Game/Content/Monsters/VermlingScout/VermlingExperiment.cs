using System.Linq;

public class VermlingExperiment : VermlingScout
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new HalfElementsShieldRetaliateTrait())
				.ToArray()
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = (stats.Traits ?? [])
				.Append(new HalfElementsShieldRetaliateTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Vermling Experiment";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<VermlingScout>();
}