using System.Collections.Generic;

public class ChieftainAMDCard07 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 11;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override int? Pierce => 1;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Conditions.Wound1];
}