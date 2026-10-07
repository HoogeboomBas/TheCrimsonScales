using Fractural.Tasks;

public class ConditionImmunityTrait : FigureTrait, IEventSubscriber
{
	private ConditionModel _conditionModel;

	public ConditionImmunityTrait(ConditionModel conditionModel)
	{
		_conditionModel = conditionModel;
	}

	public static ConditionImmunityTrait PoisonImmunityTrait()
	{
		return new ConditionImmunityTrait(Conditions.Poison1);
	}

	public static ConditionImmunityTrait WoundImmunityTrait()
	{
		return new ConditionImmunityTrait(Conditions.Wound1);
	}

	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		AbilityCmd.AddConditionImmunity(_conditionModel, this, figure);
	}

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		AbilityCmd.RemoveConditionImmunity(this);
	}
}