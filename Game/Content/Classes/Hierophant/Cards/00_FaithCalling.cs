using System.Collections.Generic;
using Fractural.Tasks;

public class FaithCalling : HierophantCardModel<FaithCalling.CardTop, FaithCalling.CardBottom>
{
	public override string Name => "Faith Calling";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 13 - 0;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
					[
						ShieldAbility.Builder().WithShieldValue(1).Build()
					]
				)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithRange(2)
				.Build()
			),

			new AbilityCardAbility(GivePrayerCardAbility(
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					GrantAbility.State grantAbilityState = state.ActionState.GetAbilityState<GrantAbility.State>(0);

					return grantAbilityState.Performed && grantAbilityState.UniqueTargetedFigures.Count == 1;
				},
				customGetTargets: (state, list) =>
				{
					GrantAbility.State grantAbilityState = state.ActionState.GetAbilityState<GrantAbility.State>(0);

					list.Add(grantAbilityState.UniqueTargetedFigures[0]);
				}
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						canApplyFunction: canApplyParameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(canApplyParameters.AbilityState.Target.Hex, 1))
							{
								if(canApplyParameters.AbilityState.Performer.AlliedWith(figure))
								{
									return true;
								}
							}

							return false;
						},
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Curse);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}
}