using System.Linq;
using Fractural.Tasks;

public class AllNegativeConditionImmunityTrait : FigureTrait
{
	public override async GDTask Activate(Figure figure)
	{
		await base.Activate(figure);

		ScenarioEvents.InflictConditionEvent.Subscribe(figure, this, parameters =>
			{
				return
					parameters.Target == figure &&
					parameters.ConditionModel?.ImmunityCompareBaseConditions != null &&
					parameters.ConditionModel.ImmunityCompareBaseConditions
						.Any(c1 => Conditions.NegativeBaseConditionModels.Contains(c1));
			},
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
				foreach(ConditionModel conditionModel in Conditions.NegativeBaseConditionModels)
				{
					parameters.AddImmunity(conditionModel);
				}
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