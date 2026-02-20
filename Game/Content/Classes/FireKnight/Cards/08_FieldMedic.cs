using System.Collections.Generic;
using Godot;

public class FieldMedic : FireKnightCardModel<FieldMedic.CardTop, FieldMedic.CardBottom>
{
	public override string Name => "Field Medic";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 12 - 8;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.61780804f, 0.7116977f)))
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(state =>
				[ModelDB.Item<FireKnightKindledTonic>(), ModelDB.Item<FireKnightScrollOfProtection>()]))
		];
	}
}