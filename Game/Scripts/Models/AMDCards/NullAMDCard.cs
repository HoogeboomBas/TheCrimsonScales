public class NullAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 18;

	public override bool Reshuffles => true;
	public override AMDCardType Type => AMDCardType.Null;
}