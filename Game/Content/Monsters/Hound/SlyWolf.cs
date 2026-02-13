using System.Linq;

public class SlyWolf : Hound
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount - 1),
				Traits = (stats.Traits ?? [])
				.Append(new PermanentConditionTrait(Conditions.Invisible))
				.ToArray()
			})
			.ToArray();

	public override string Name => "Sly Wolf";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<Hound>();
}