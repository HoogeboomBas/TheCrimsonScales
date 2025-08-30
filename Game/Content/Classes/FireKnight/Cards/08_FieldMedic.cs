using System.Collections.Generic;

public class FieldMedic : FireKnightCardModel<FieldMedic.CardTop, FieldMedic.CardBottom>
{
	public override string Name => "Field Medic";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 12 - 8;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder().WithHealValue(3)
				.WithRange(2)
				.WithAfterHealPerformedSubscription(
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target,
								Conditions.Strengthen);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<KindledTonic>(), ModelDB.Item<ScrollOfProtection>()]))
		];
	}
}