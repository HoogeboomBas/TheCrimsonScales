using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;

public class MutualAid : FireKnightLevelUpCardModel<MutualAid.CardTop, MutualAid.CardBottom>
{
	public override string Name => "Mutual Aid";
	public override int Level => 6;
	public override int Initiative => 44;
	protected override int AtlasIndex => 6;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					AttackAbility.Builder().WithDamage(3).Build()
				])
				.WithRange(1)
				.WithTargets(2)
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.Build()),
			new AbilityCardAbility(GiveFireKnightItemAbility(
				[ModelDB.Item<FireproofHelm>(), ModelDB.Item<ScrollOfCharisma>(), ModelDB.Item<PikeHook>()],
				customGetTargets: (state, list) =>
				{
					List<ActionState> grantAbilityActionStates = state.ActionState.GetAbilityState<GrantAbility.State>(0).GrantAbilityActionStates;
					list.AddRange(grantAbilityActionStates
						.Where(a => a.Performer != state.Performer)
						.Select(a => a.Performer)
					);
				},
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					List<ActionState> grantAbilityActionStates = state.ActionState.GetAbilityState<GrantAbility.State>(0).GrantAbilityActionStates;
					bool canPerform = grantAbilityActionStates.Count > 1 && grantAbilityActionStates[0].GetAbilityState<AttackAbility.State>(0).Performed &&
						grantAbilityActionStates[1].GetAbilityState<AttackAbility.State>(0).Performed &&
						grantAbilityActionStates[0].GetAbilityState<AttackAbility.State>(0).Target ==
						grantAbilityActionStates[1].GetAbilityState<AttackAbility.State>(0).Target;
					if(canPerform)
                    {
                        await AbilityCmd.GainXP(state.Performer, 1);
                    }

					return canPerform;
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(1)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => true,
						async parameters =>
						{
							await AbilityCmd.GenericChoice(parameters.Performer,
							[
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((HealAbility.State)parameters.AbilityState).SetTarget(Target.Self);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new TextEffectButton.Parameters("Self"),
									effectInfoViewParameters: new TextEffectInfoView.Parameters("Self"),
									effectType: EffectType.Selectable
								),
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((HealAbility.State)parameters.AbilityState).SetTarget(Target.Allies | Target.TargetAll);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new TextEffectButton.Parameters("Allies"),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Targets)}all allies, {Icons.Inline(Icons.Range)} 1"),
									effectType: EffectType.Selectable
								),
							], hintText: "Choose the figures to heal");
						}
					)
				)
				.WithAfterHealPerformedSubscription(
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target,
								Conditions.Strengthen);
						}
					)
				)
				.Build()),
		];
	}
}