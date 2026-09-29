using System.Linq;

public class AncientArtilleryScenario056 : AncientArtillery
{
	public override MonsterStats[] NormalLevelStats =>
		base.NormalLevelStats
			.Select(stats => stats with
			{
				Range = 4
			})
			.ToArray();

	public override MonsterStats[] EliteLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Range = 4
			})
			.ToArray();

	public override MonsterModel ParentMonsterModel => ModelDB.Monster<AncientArtillery>();
}