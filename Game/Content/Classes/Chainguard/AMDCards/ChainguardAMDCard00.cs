using System.Collections.Generic;

public class ChainguardAMDCard00 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 0;

	public override int? GetValue(AttackAbility.State state) => 1;

	public override List<ConditionModel> GetConditionModels(AttackAbility.State state) => [Chainguard.Shackle];
}