using System.Collections.Generic;
using Fractural.Tasks;

public class FireKnightRescueShield : FireKnightItem
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

					if(parameters.PotentialAbilityState is AttackAbility.State attackAbilityState &&
					   attackAbilityState.SingleTargetConditionModels.Count > 0)
					{
						List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
							new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
						foreach(ConditionModel conditionModel in attackAbilityState.SingleTargetConditionModels)
						{
							subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
								applyFunction: async applyParameters =>
								{
									attackAbilityState.SingleTargetRemoveCondition(conditionModel);

									await GDTask.CompletedTask;
								},
								effectType: EffectType.SelectableMandatory,
								effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(conditionModel)),
								effectInfoViewParameters: new TextEffectInfoView.Parameters(
									$"Prevent {Icons.Inline(Icons.GetCondition(conditionModel))}")
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