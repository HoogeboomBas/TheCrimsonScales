public abstract class EmpowerAMDCard : AMDCard
{
	public override bool RemoveAfterDraw => true;

	public EmpowerAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}