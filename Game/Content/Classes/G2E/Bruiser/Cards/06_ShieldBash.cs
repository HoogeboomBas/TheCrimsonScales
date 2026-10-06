using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ShieldBash : BruiserCardModel<ShieldBash.CardTop, ShieldBash.CardBottom>
{
	public override string Name => "Shield Bash";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 6;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithConditions(Conditions.Stun)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveSquare(this, new Vector2(0.61780804f, 0.77249503f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build())
		];

		public override bool Round => true;
	}
}