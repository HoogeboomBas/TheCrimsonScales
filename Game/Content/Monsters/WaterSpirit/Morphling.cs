using System.Linq;

public class Morphling : WaterSpirit
{
	public override MonsterStats[] BossLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount * 2,
				Traits = (stats.Traits ?? [])
				.Append(new ForcedMovementImmunityTrait())
				.Append(new AllNegativeConditionImmunityTrait())
				.Append(new IgnoreDifficultTerrainTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Morphling";

	public override int MaxStandeeCount => 1;
}