using System.Collections.Generic;
using Godot;

public class ScorchedPath : RimehearthCardModel<ScorchedPath.CardTop, ScorchedPath.CardBottom>
{
	public override string Name => "Scorched Path";
	public override int Level => 1;
	public override int Initiative => 51;
	protected override int AtlasIndex => 6;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214308f, 0.17829336f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.51037705f, 0.27700832f), EnhancementCostType.MultiTarget))
				.WithConditions(Conditions.Wound1)
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

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62115484f, 0.6550556f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(1);
							applyParameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Fire, effectInfoText: $"{Icons.Inline(Icons.Heal)}2"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];
	}
}