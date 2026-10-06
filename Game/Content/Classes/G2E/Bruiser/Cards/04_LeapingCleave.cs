using System.Collections.Generic;
using Godot;

public class LeapingCleave : BruiserCardModel<LeapingCleave.CardTop, LeapingCleave.CardBottom>
{
	public override string Name => "Leaping Cleave";
	public override int Level => 1;
	public override int Initiative => 54;
	protected override int AtlasIndex => 4;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithRange(1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red)
				]))
				.Build()),
		];

		public override int XP => 1;
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.61780804f, 0.77249503f)))
				.WithMoveType(MoveType.Jump)
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushSquare(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithRange(1)
				.Build())
		];

	}
}