using System.Collections.Generic;

public class ChainguardAMDCard09 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 2;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => [Chainguard.Shackle];

	public ChainguardAMDCard09(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}