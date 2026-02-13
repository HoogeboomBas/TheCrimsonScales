using System.Linq;

public class BoneArcher : LivingBones
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Range = 3,
				Traits = (stats.Traits ?? [])
				.Where(trait => trait is not TargetsTrait)
				.ToArray()
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Range = 3,
				Traits = (stats.Traits ?? [])
				.Where(trait => trait is not TargetsTrait)
				.ToArray()
			})
			.ToArray();

	public override string Name => "Bone Archer";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<LivingBones>();
}