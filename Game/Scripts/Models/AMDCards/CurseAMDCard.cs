public class CurseAMDCard : AMDCard
{
	public override bool RemoveAfterDraw => true;
	public override AMDCardType Type => AMDCardType.Null;
	public override int? Value => null;

	public CurseAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}