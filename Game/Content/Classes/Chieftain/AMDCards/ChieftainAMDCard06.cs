public class ChieftainAMDCard06 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => state.Performer is Summon;

	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 1;

	public ChieftainAMDCard06(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}