using System.Collections.Generic;
using Godot;

public class ViolentOutlash : AmberAegisCardModel<ViolentOutlash.CardTop, ViolentOutlash.CardBottom>
{
	public override string Name => "Violent Outlash";
	public override int Level => 3;
	public override int Initiative => 63;
	protected override int AtlasIndex => 17;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				]))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeChoiceElement([Element.Fire, Element.Earth],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
				]))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeChoiceElement([Element.Fire, Element.Earth],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(2);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}2")))
				.Build())
		];

		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62128145f, 0.6736648f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.Build())
		];

		//TODO: Create Fire or Earth
	}
}