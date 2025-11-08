using System.Collections.Generic;
using System;

public class ChainguardAMDCard03 : AMDCard
{
	public override bool Rolling(AttackAbility.State state) => true;
	
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;

	public override Func<AttackAbility.State, List<Ability>> GetAbilities => 
		state =>
		[
			RetaliateAbility.Builder().WithRetaliateValue(1).Build()
		];

	public ChainguardAMDCard03(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}