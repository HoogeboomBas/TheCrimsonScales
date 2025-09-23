using Fractural.Tasks;
using Godot;

public class ConditionImmunityTrait : FigureTrait
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

	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.InflictConditionEvent.Subscribe(figure, this,
			parameters => parameters.Target == figure && parameters.Condition.ImmunityCompareBaseCondition == _conditionModel.ImmunityCompareBaseCondition,
			async parameters =>
			{
				parameters.SetPrevented(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(figure, this,
			parameters => parameters.Figure == figure,
			parameters =>
			{
				parameters.AddImmunity(_conditionModel);
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.InflictConditionEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(figure, this);
	}
}