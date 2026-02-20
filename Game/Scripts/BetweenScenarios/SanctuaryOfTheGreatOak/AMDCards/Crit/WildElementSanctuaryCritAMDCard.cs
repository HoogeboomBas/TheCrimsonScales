using System.Collections.Generic;

public class WildElementSanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			rolling: true);

	protected override int AtlasIndex => 4;

	public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.InfuseWild()];
}