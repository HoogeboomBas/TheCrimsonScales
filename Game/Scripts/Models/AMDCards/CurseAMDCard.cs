public class CurseAMDCard : DefaultOtherAMDCardModel
{
	protected override int AtlasIndex => 2;

	public override bool Reshuffles => true;
	public override bool RemoveAfterDraw => true;
	public override AMDCardType Type => AMDCardType.Null;
}