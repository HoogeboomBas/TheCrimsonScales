using System.Linq;

public class Lavalite : SavvasLavaflow
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount + 1),
				Traits = (stats.Traits ?? [])
				.Append(new ConditionImmunityTrait(Conditions.Stun))
				.Append(new ConditionImmunityTrait(Conditions.Disarm))
				.Append(new ConditionImmunityTrait(Conditions.Muddle))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Lavalite";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<SavvasLavaflow>();
}