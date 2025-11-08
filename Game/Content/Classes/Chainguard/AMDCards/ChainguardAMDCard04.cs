public class ChainguardAMDCard04 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => true;
	
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;
	public override int? Swing => 3;

	public ChainguardAMDCard04(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}