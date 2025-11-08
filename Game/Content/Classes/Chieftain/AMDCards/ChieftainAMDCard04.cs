using System.Collections.Generic;

public class ChieftainAMDCard04 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;
	public override int? Push => 1;
	public override List<ConditionModel> ConditionModels(AttackAbility.State state) => [Conditions.Immobilize];

	public ChieftainAMDCard04(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}