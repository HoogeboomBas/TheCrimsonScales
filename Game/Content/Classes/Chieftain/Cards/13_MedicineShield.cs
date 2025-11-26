using System.Collections.Generic;

public class MedicineShield : ChieftainCardModel<MedicineShield.CardTop, MedicineShield.CardBottom>
{
	public override string Name => "Medicine Shield";
	public override int Level => 2;
	public override int Initiative => 19;
	protected override int AtlasIndex => 13;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustHealValue(2);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Heal)}"))
				)
				.Build())
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => 
				[
					HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build(),
					ShieldAbility.Builder().WithShieldValue(2).Build()
				])
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithRange(3)
				.Build()
			)
		];

		protected override int XP => 2;
		protected override bool Round => true;
		protected override bool Loss => true;
	}
}