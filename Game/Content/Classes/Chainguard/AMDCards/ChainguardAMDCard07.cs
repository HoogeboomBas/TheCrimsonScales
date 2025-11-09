using System.Collections.Generic;
using System;

public class ChainguardAMDCard07 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 1;

	public override List<Ability> Abilities =>
	[
		CreateTrapAbility.Builder().WithDamage(2).WithRange(2).Build()
	];

	public ChainguardAMDCard07(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}