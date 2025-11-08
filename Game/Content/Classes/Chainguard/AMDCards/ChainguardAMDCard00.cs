using System.Collections.Generic;

public class ChainguardAMDCard00 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 1;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => [Chainguard.Shackle];

	public ChainguardAMDCard00(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}