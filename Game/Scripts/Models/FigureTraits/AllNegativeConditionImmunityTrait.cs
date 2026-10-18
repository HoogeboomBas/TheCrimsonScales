using Fractural.Tasks;

public class AllNegativeConditionImmunityTrait : FigureTrait, IEventSubscriber
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		AbilityCmd.AddAllNegativeConditionImmunity(this, figure);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		AbilityCmd.RemoveConditionImmunity(this);
	}
}