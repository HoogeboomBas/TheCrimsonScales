using System.Collections.Generic;
using System;
using Fractural.Tasks;

public class ChieftainAMDCard09 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => true;

	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;
	public override int? Pierce => 2;
	public override bool IgnoreRetaliate => true;

	public ChieftainAMDCard09(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}