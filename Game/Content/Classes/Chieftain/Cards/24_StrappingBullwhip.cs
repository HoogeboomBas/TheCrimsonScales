using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class StrappingBullwhip : ChieftainCardModel<StrappingBullwhip.CardTop, StrappingBullwhip.CardBottom>
{
	public override string Name => "Strapping Bullwhip";
	public override int Level => 7;
	public override int Initiative => 29;
	protected override int AtlasIndex => 24;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red)
					]
				))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						canApplyFunction: canApplyParameters => true,
						applyFunction: async parameters =>
						{
							int distance = RangeHelper.Distance(parameters.Performer.Hex, parameters.AbilityState.Target.Hex);
							parameters.AbilityState.SingleTargetAdjustAttackValue(distance);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer == state.Performer &&
						                      canApplyParameters.AbilityState.AbilityRangeType == RangeType.Melee,
						async applyParameters =>
						{
							if(Chieftain.GetIsMounted(state.Performer))
							{
								applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);
								applyParameters.AbilityState.SingleTargetAdjustPierce(2);

								await state.ActionState.RequestDiscardOrLose();
							}
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}