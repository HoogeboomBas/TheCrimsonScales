using System.Collections.Generic;

public class WildElementSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	protected override int AtlasIndex => 4;

	public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.InfuseWild()];
}