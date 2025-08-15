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
			new AbilityCardAbility(new AttackAbility(2, range: 4)),
			new AbilityCardAbility(new PullSelfAbility(4, customGetTargets: (state, targets) =>
			{
				AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
				targets.AddRange(attackAbilityState.UniqueTargetedFigures);
			}))
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3, MoveType.Jump))
		];
	}
}