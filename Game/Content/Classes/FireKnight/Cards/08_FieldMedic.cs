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
			new AbilityCardAbility(new HealAbility(3, range: 2,
				afterHealPerformedSubscriptions:
				[
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target, Conditions.Strengthen);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				]
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(2)),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<KindledTonic>(), ModelDB.Item<ScrollOfProtection>()]))
		];
	}
}