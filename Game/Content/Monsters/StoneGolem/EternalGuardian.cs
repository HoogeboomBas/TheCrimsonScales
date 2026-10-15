using System.Linq;

public class EternalGuardian : StoneGolem
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats
			.Select(stats => stats with
			{
				Traits = [ConditionImmunityTrait.WoundImmunityTrait()]
			})
			.ToArray();

	public override string Name => "Eternal Guardian";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<StoneGolem>();
}