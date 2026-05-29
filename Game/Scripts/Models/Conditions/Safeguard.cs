using System.Collections.Generic;
using Fractural.Tasks;

public class Safeguard : ConditionModel
{
	public override string Name => "Safeguard";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Safeguard.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		ScenarioEvents.InflictConditionsEvent.Subscribe(condition,
			parameters => parameters.Target == condition.Owner && parameters.ConditionModels.Count > 0,
			async parameters =>
			{
				condition.Flash();

				List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
					new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
				foreach(ConditionModel conditionModel in parameters.ConditionModels)
				{
					subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
						applyFunction: async applyParameters =>
						{
							parameters.PreventCondition(conditionModel);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.SelectableMandatory,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(conditionModel)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Prevent {Icons.Inline(Icons.GetCondition(conditionModel))}")
					));
				}

				await AbilityCmd.GenericChoice(condition.Owner, subscriptions, hintText: "Select a condition to prevent");

				await AbilityCmd.RemoveCondition(condition, parameters.PotentialAbilityState);
			},
			order: 100
		);
	}

	public override async GDTask OnRemoved(Condition condition)
	{
		await base.OnRemoved(condition);

		ScenarioEvents.InflictConditionsEvent.Unsubscribe(condition);
	}
}