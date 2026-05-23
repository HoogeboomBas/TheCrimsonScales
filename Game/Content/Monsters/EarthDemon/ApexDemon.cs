using System.Linq;

public class ApexDemon : EarthDemon
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount - 1),
				Move = stats.Move + 2,
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.Append(new ConditionImmunityTrait(Conditions.Wound1))
				.Append(new ConditionImmunityTrait(Conditions.Curse))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Apex Demon";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<EarthDemon>();
}