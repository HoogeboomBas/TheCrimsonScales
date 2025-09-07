public class CritAMDCard : AMDCard
{
	public override bool Reshuffles => true;
	public override AMDCardType Type => AMDCardType.Crit;
	public override int? Value => null;

	public CritAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}