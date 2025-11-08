using System.Collections.Generic;
using System;

public class ChieftainAMDCard03 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => -2;

	public override Func<AttackAbility.State, List<Ability>> GetAbilities => 
		state =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Self).Build()
		];

	public ChieftainAMDCard03(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}