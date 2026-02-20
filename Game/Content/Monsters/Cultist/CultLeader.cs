using System.Linq;

public class CultLeader : Cultist
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount + 1),
				Move = stats.Move + 2,
				Attack = stats.Attack + 3,
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ConditionImmunityTrait(Conditions.Immobilize))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Cult Leader";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<Cultist>();
}