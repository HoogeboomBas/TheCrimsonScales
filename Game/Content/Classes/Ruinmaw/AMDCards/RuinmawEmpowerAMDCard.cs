using System.Linq;

public class RuinmawEmpowerAMDCard : EmpowerAMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value => 1;
	public override bool Rolling => true;
	public IHasEmpower Owner;
	//TODO: Change so it takes in state
	//TODO: Add Push 1 once amds are implemeneted

	public RuinmawEmpowerAMDCard(IHasEmpower figure)
		: base("res://Content/Classes/Ruinmaw/AMDCards/AMDCards.png", 5, 3, 2)
	{
		Owner = figure;
    }

	public override void Drawn()
	{
		base.Drawn();
		Owner.RemainingEmpowerCount++;
	}
}