using System.Collections.Generic;
using Fractural.Tasks;

public class RescueShield : FireKnightItem
{
	public override string Name => "Rescue Shield";
	public override int ItemNumber => 2;
	protected override int AtlasIndex => 10 - 2;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeSufferDamage(
			canApply: parameters => parameters.FromAttack && parameters.Figure == Owner && parameters.WouldSufferDamage,
			apply: async parameters =>
			{
				await Use(async user =>
				{
					parameters.AdjustShield(2);

					if(((AttackAbility.State)parameters.AbilityState).SingleTargetConditionModels.Count > 0)
					{
						List<ScenarioEvents.GenericChoice.Subscription> subscriptions = new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
						foreach(ConditionModel conditionModel in ((AttackAbility.State)parameters.AbilityState).SingleTargetConditionModels)
						{
							subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
								applyFunction: async applyParameters =>
								{
									((AttackAbility.State)parameters.AbilityState).SingleTargetRemoveCondition(conditionModel);

									await GDTask.CompletedTask;
								},
								effectType: EffectType.SelectableMandatory,
								effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(conditionModel)),
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"Prevent {Icons.Inline(Icons.GetCondition(conditionModel))}")
							));
						}

						await AbilityCmd.GenericChoice(user, subscriptions);
					}

					await GDTask.CompletedTask;
				});
			}
		);
	}
}