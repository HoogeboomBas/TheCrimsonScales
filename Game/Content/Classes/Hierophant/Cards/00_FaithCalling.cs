using System.Collections.Generic;
using Fractural.Tasks;

public class FaithCalling : HierophantCardModel<FaithCalling.CardTop, FaithCalling.CardBottom>
{
	public override string Name => "Faith Calling";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 29 - 0;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new GrantAbility(
				figure =>
				[
					ShieldAbility.Builder().WithShieldValue(1).Build()
				], target: Target.Allies | Target.TargetAll, range: 2
			)),

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
			new AbilityCardAbility(new AttackAbility(1, range: 3,
				afterTargetConfirmedSubscriptions:
				[
					ScenarioEvent<ScenarioEvents.AttackAfterTargetConfirmed.Parameters>.Subscription.New(
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
				]
			))
		];
	}
}