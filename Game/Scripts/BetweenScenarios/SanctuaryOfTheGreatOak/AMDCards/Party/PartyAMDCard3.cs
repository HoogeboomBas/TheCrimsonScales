public class PartyAMDCard3 : PartyAMDCardModel
{
	public override string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, AMDCardType.Crit,
			extraText: $"+1{Icons.Inline(Icons.Targets)} adjacent to this target",
			rolling: true);

	protected override int AtlasIndex => 8;

	//TODO: Actually implement this effect
	public override int? AddedTargets => 1;
}