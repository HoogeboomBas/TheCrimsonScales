using System.Collections.Generic;
using System.Linq;

public class TerribleTwoBanditGuard : BanditGuard
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Attack = stats.Attack + 1,
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.Append(ConditionImmunityTrait.WoundImmunityTrait())
				.Append(ConditionImmunityTrait.PoisonImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "Terrible Two (Bandit Guard)";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<BanditGuard>();
}