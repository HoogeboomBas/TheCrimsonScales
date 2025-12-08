using System.Collections.Generic;

public class ChieftainAMDCard04 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 4;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override int? Push => 1;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Conditions.Immobilize];
}