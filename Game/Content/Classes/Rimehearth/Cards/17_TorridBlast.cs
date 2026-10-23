using System.Collections.Generic;
using Godot;

public class TorridBlast : RimehearthCardModel<TorridBlast.CardTop, TorridBlast.CardBottom>
{
	public override string Name => "Torrid Blast";
	public override int Level => 4;
	public override int Initiative => 31;
	protected override int AtlasIndex => 17;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Wound1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Fire, effectInfoText: $"{Icons.Inline(Icons.Move)}3"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.7731072f), EnhancementCostType.MultiTarget))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							((AttackAbility.State)applyParameters.AbilityState).AbilitySetAOEPattern(new AOEPattern(
								[
									new AOEHex(Vector2I.Zero, AOEHexType.Gray),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
								]
							));

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetAOEPattern(new AOEPattern(
							[
								new AOEHex(Vector2I.Zero, AOEHexType.Gray),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
							]
						)))}")
					)
				)
				.Build()),
		];
	}
}