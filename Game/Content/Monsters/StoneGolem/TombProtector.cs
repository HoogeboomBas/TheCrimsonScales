public class TombProtector : StoneGolem
{
	public override MonsterStats[] NamedLevelStats =>
		base.EliteLevelStats;

	public override string Name => "Tomb Protector";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<StoneGolem>();
}