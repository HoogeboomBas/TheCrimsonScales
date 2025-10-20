using Fractural.Tasks;

public class Infect : ConditionModel
{
	public override string Name => "Infect";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Infect.svg";
	public override bool RemovedByHeal => true;
	public override bool IsNegative => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);
	}

	public override async GDTask Remove()
	{
		await base.Remove();
	}
}