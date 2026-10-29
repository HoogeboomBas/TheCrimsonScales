using System.Collections.Generic;
using Godot;

public class RapidCalcification : ShardrenderCardModel<RapidCalcification.CardTop, RapidCalcification.CardBottom>
{
	public override string Name => "Rapid Calcification";
	public override int Level => 7;
	public override int Initiative => 48;
	protected override int AtlasIndex => 24;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithConditions(Conditions.Immobilize)
				.Build()),
			new AbilityCardAbility(
				MoveCharacterTokenBackAbility(new DynamicInt<OtherAbility.State>(state =>
						state.ActionState.GetAbilityState<AttackAbility.State>(1).UniqueTargetedFigures.Count))
					.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 1))
					.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62120676f, 0.67176825f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build())
		];
	}
}