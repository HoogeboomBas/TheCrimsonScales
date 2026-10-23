using System.Collections.Generic;
using Godot;

public class PressureWave : RimehearthCardModel<PressureWave.CardTop, PressureWave.CardBottom>
{
	public override string Name => "Pressure Wave";
	public override int Level => 8;
	public override int Initiative => 16;
	protected override int AtlasIndex => 26;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
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
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.Consume([Element.Fire, Element.Ice])],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilitySetHasAdvantage();

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters("advantage")))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Chill) && parameters.AbilityState.Target.HasWound(),
						async parameters =>
						{
							if(await AbilityCmd.RemoveChillStack(parameters.AbilityState.Target) &
							   await AbilityCmd.RemoveWound(parameters.AbilityState.Target))
							{
								parameters.AbilityState.SingleTargetAddCondition(Conditions.Brittle);
							}
						}, EffectType.Selectable,
						effectButtonParameters: new TextEffectButton.Parameters(
							$"{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}{Icons.Inline(Icons.GetCondition(Conditions.Chill))}"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} and one {Icons.Inline(Icons.GetCondition(Conditions.Chill))} token from the target to add {Icons.Inline(Icons.GetCondition(Conditions.Brittle))}")))
				.Build()),
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62115484f, 0.69609886f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(2);
							applyParameters.AbilityState.AdjustMoveType(MoveType.Jump);
							applyParameters.AbilityState.SetCustomValue(this, "FireConsumed", true);

							await AbilityCmd.InfuseElement(applyParameters.AbilityState, [Element.Fire, Element.Ice]);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+2{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}, {Icons.Inline(Icons.GetElement([Element.Fire, Element.Ice]))}")
					)
				)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(async state =>
				{
					return !state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "FireConsumed") &&
					       await AbilityCmd.AskConsumeElement(state.Performer, Element.Ice,
						       effectInfoText: $"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} self");
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];
	}
}