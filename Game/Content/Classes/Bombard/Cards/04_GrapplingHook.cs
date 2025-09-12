using System.Collections.Generic;

public class GrapplingHook : BombardCardModel<GrapplingHook.CardTop, GrapplingHook.CardBottom>
{
	public override string Name => "Grappling Hook";
	public override int Level => 1;
	public override int Initiative => 68;
	protected override int AtlasIndex => 4;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).WithRange(4).Build()),
			new AbilityCardAbility(PullSelfAbility.Builder()
				.WithPullSelfValue(4)
				.WithCustomGetTargets((state, targets) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					targets.AddRange(attackAbilityState.UniqueTargetedFigures);
				})
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}