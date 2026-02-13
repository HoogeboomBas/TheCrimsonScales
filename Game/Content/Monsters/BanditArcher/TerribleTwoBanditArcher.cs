using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class TerribleTwoBanditArcher : BanditArcher
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount + 2),
				Traits = (stats.Traits ?? [])
				.Append(new TargetsTrait(2))
				.Append(new JumpTrait())
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Muddle))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ConditionImmunityTrait(Conditions.Curse))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Terrible Two (Bandit Archer)";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<BanditArcher>();
}