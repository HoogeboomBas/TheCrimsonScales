using Fractural.Tasks;
using Godot;
using System.Linq;

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

	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.InflictConditionEvent.Subscribe(figure, this,
			parameters =>
				parameters.Target == figure &&
				AbilityCmd.CheckImmunity(parameters.ConditionModel, _conditionModel),
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

	public override async GDTask Deactivate(Figure figure)
	{
		await base.Deactivate(figure);

		ScenarioEvents.InflictConditionEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(figure, this);
	}
}