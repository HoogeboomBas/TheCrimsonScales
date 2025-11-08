using System.Collections.Generic;

public class ChainguardAMDCard06 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 1;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => 
		state.Target.HasCondition(Chainguard.Shackle) ? [Conditions.Disarm] : null;

	public ChainguardAMDCard06(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}