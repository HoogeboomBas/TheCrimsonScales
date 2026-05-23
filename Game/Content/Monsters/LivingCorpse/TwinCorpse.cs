using System.Linq;

public class TwinCorpse : LivingCorpse
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Health = stats.Health * (CharacterCount - 1)
			})
			.ToArray();

	public override string Name => "Twin Corpse";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<LivingCorpse>();
}