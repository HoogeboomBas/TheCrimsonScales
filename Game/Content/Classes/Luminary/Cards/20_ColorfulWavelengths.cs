using System.Collections.Generic;
using Godot;

public class ColorfulWavelengths : LuminaryCardModel<ColorfulWavelengths.CardTop, ColorfulWavelengths.CardBottom>
{
	public override string Name => "Colorful Wavelengths";
	public override int Level => 5;
	public override int Initiative => 83;
	protected override int AtlasIndex => 20;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.6192083f, 0.13470992f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(new AOEPattern(
								[
									new AOEHex(Vector2I.Zero, AOEHexType.Gray),
									new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
									new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
								]
							));

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Ice);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Add the shown area of effect")
					)
				)
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(2);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Fire);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}2")
					),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPush(2);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Dark);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}2")
					),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Curse);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Curse))}")
					)
				])
				.Build())
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(PerformFreeGlow())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
		[
			CardElementInfusion.Infuse(Element.Fire),
			CardElementInfusion.Infuse(Element.Ice),
			CardElementInfusion.Infuse(Element.Light),
			CardElementInfusion.Infuse(Element.Dark)
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}