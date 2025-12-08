using System.Collections.Generic;

public class ChieftainAMDCard08 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 8;

	public override int? GetValue(AttackAbility.State state) => 1;

	public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];
}