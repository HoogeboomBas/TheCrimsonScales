public class BlessAMDCard : AMDCard
{
	public override bool RemoveAfterDraw => true;
	public override AMDCardType Type => AMDCardType.Crit;
	public override int? Value => null;

	public BlessAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}