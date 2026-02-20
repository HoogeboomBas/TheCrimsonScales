using System.Linq;

public class KingCobra : GiantViper
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount * 2 + 2),
				Traits = (stats.Traits ?? [])
				.Append(ConditionImmunityTrait.PoisonImmunityTrait())
				.ToArray()
			})
			.ToArray();

	public override string Name => "King Cobra";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<GiantViper>();
}