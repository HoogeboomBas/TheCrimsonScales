public class ArchaicGolem : StoneGolem
{
	public override string Name => "Archaic Golem";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<StoneGolem>();
}