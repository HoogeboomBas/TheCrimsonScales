public class ChieftainAMDCard10 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 1;

	public ChieftainAMDCard10(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}