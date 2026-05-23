using System.Linq;

public class ShadowDemon : NightDemon
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * CharacterCount,
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.Append(new ConditionImmunityTrait(Conditions.Curse))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Shadow Demon";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<NightDemon>();
}