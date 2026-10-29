using System.Collections.Generic;
using Godot;

public class SplinterBurst : ShardrenderCardModel<SplinterBurst.CardTop, SplinterBurst.CardBottom>
{
	public override string Name => "Splinter Burst";
	public override int Level => 1;
	public override int Initiative => 50;
	protected override int AtlasIndex => 13;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.33543774f, 0.27720013f)))
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithPierce(2)
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
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62115484f, 0.68124044f)))
				.WithDuringMovementSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringMovement.Parameters>(async parameters =>
					{
						parameters.AbilityState.AdjustMoveValue(1);
						parameters.AbilityState.AdjustMoveType(MoveType.Jump);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithRange(1)
				.Build())
		];
	}
}