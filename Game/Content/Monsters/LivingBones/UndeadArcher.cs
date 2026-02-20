using System.Linq;

public class UndeadArcher : LivingBones
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
			.Select((stats, index) => stats with
			{
				Range = 3,
				Traits = index == 0
					? (stats.Traits ?? [])
					.Where(trait => trait is not TargetsTrait)
					.ToArray()
					: (stats.Traits ?? [])
					.Where(trait => trait is not TargetsTrait)
					.Append(new TargetsTrait(2))
					.ToArray()
			})
			.ToArray();

	public override string Name => "Undead Archer";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<LivingBones>();
}