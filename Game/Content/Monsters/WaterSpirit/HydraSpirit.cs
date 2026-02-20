using System.Linq;

public class HydraSpirit : WaterSpirit
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Traits = (stats.Traits ?? [])
					.Append(new ConditionImmunityTrait(Conditions.Stun))
					.Append(new ConditionImmunityTrait(Conditions.Disarm))
					.Append(ConditionImmunityTrait.PoisonImmunityTrait())
					.Append(new ConditionImmunityTrait(Conditions.Immobilize))
					.ToArray()
			})
			.ToArray();

	public override string Name => "Hydra Spirit";

	public override int MaxStandeeCount => 1;
}