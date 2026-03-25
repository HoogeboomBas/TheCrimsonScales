using System.Collections.Generic;

public class ChieftainAMDCard00 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 0;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Conditions.Poison1];
}