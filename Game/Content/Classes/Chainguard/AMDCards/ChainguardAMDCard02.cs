using System.Collections.Generic;
using System;

public class ChainguardAMDCard02 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => true;
	
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;

	public override List<Ability> Abilities =>
	[
		ShieldAbility.Builder().WithShieldValue(1).Build()
	];

	public ChainguardAMDCard02(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}