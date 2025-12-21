public class CritAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 19;

	public override bool Reshuffles => true;
	public override AMDCardType Type => AMDCardType.Crit;
}