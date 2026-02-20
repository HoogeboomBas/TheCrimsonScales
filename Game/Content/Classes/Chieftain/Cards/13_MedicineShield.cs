using System.Collections.Generic;
using Godot;

public class MedicineShield : ChieftainCardModel<MedicineShield.CardTop, MedicineShield.CardBottom>
{
	public override string Name => "Medicine Shield";
	public override int Level => 2;
	public override int Initiative => 19;
	protected override int AtlasIndex => 13;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.4407086f, 0.23097177f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6508312f, 0.22997175f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build(),
					ShieldAbility.Builder().WithShieldValue(2).Build()
				])
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithRange(3)
				.Build()
			)
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}
}