using System;
using System.Collections.Generic;

public class ChainguardAMDCard08 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => true;
	
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;

	public override List<Ability> Abilities =>
	[
		HealAbility.Builder().WithHealValue(1).Build()
	];

	public ChainguardAMDCard08(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}