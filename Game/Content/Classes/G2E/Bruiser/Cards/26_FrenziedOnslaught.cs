using System.Collections.Generic;
using Godot;

public class FrenziedOnslaught : BruiserCardModel<FrenziedOnslaught.CardTop, FrenziedOnslaught.CardBottom>
{
	public override string Name => "Frenzied Onslaught";
	public override int Level => 8;
	public override int Initiative => 19;
	protected override int AtlasIndex => 26;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.49855936f, 0.17132856f)))
				.WithConditions(Conditions.Poison2)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.51042974f, 0.26951215f)))
				.WithConditions(Conditions.Poison1)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters =>
						{
							AttackAbility.State previousAttackState = parameters.AbilityState.ActionState.GetAbilityState<AttackAbility.State>(0);
							return previousAttackState.Performed &&
							       previousAttackState.UniqueTargetedFigures.Contains(parameters.AbilityState.Target);
						},
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.510711f, 0.773974f)))
				.WithConditions(Conditions.Poison1)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build())
		];
	}
}