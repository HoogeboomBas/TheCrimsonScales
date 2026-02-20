public class BlessAMDCard : DefaultOtherAMDCardModel
{
	protected override int AtlasIndex => 3;

	public override bool Reshuffles => true;
	public override bool RemoveAfterDraw => true;
	public override AMDCardType Type => AMDCardType.Crit;
}