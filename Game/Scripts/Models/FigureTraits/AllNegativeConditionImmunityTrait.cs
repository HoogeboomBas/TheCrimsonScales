using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class AllNegativeConditionImmunityTrait : FigureTrait
{
	private static readonly List<ConditionModel> NegativeConditionModels =
	[
		Conditions.Poison1,
		Conditions.Wound1,
		Conditions.Muddle,
		Conditions.Immobilize,
		Conditions.Disarm,
		Conditions.Stun,
		Conditions.Curse,
	];

	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.InflictConditionEvent.Subscribe(figure, this, parameters =>
			{
				return parameters.Target == figure &&
					parameters.Condition?.ImmunityCompareBaseCondition != null &&
					NegativeConditionModels != null &&
					parameters.Condition.ImmunityCompareBaseCondition
						.Any(c1 => NegativeConditionModels.Contains(c1));
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
				foreach(ConditionModel conditionModel in NegativeConditionModels)
				{
					parameters.AddImmunity(conditionModel);
				}
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