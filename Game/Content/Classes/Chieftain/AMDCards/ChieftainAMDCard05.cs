public class ChieftainAMDCard05 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) =>
		state.Performer is Character performer ? performer.Summons.Count : 
		state.Performer is Summon summon ? summon.CharacterOwner.Summons.Count : 
		((Character)state.Authority).Summons.Count;

	public ChieftainAMDCard05(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}