using System.Collections.Generic;

public class ChieftainAMDCard00 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => [Conditions.Poison1];

	public ChieftainAMDCard00(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}