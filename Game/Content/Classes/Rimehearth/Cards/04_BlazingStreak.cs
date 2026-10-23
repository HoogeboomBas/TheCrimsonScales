using System.Collections.Generic;
using Godot;

public class BlazingStreak : RimehearthCardModel<BlazingStreak.CardTop, BlazingStreak.CardBottom>
{
	public override string Name => "Blazing Streak";
	public override int Level => 1;
	public override int Initiative => 33;
	protected override int AtlasIndex => 4;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscriptions(
					[
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AbilityAdjustAttackValue(1);

								await AbilityCmd.GainXP(applyParameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
						),
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.AbilityAddCondition(Conditions.Wound1);

								await AbilityCmd.GainXP(applyParameters.Performer, 1);
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")
						)
					]
				)
				.Build()),
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6186266f, 0.6404432f)))
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Fire, effectInfoText: $"{Icons.Inline(Icons.Retaliate)}2"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
					state.ActionState.SetOverrideRound();
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Wound1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 1))
				.Build())
		];
	}
}