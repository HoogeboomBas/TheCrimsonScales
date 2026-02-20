using System.Collections.Generic;
using Godot;

public class GrapplingHook : BombardCardModel<GrapplingHook.CardTop, GrapplingHook.CardBottom>
{
	public override string Name => "Grappling Hook";
	public override int Level => 1;
	public override int Initiative => 68;
	protected override int AtlasIndex => 4;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.5008511f, 0.19675682f)))
				.WithRange(4)
				.Build()),

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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.52561843f, 0.76499504f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}