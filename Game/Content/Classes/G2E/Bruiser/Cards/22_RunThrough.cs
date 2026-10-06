using System.Collections.Generic;
using Godot;

public class RunThrough : BruiserCardModel<RunThrough.CardTop, RunThrough.CardBottom>
{
	public override string Name => "Run Through";
	public override int Level => 6;
	public override int Initiative => 12;
	protected override int AtlasIndex => 22;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.5104704f, 0.24021162f)))
				.WithConditions(Conditions.Wound1)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4975778f, 0.33862433f)))
				.WithConditions(Conditions.Wound2)
				.Build())
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.5243444f, 0.7206349f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}