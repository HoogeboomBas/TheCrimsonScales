public class NullAMDCard : AMDCard
{
	public override bool Reshuffles => true;
	public override AMDCardType Type => AMDCardType.Null;
	public override int? Value => null;

	public NullAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}