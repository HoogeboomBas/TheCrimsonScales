using System.Collections.Generic;
using System;

public class ChieftainAMDCard07 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;
	public override int? Pierce => 1;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => [Conditions.Wound1];

	public ChieftainAMDCard07(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}