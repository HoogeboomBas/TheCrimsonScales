using System.Collections.Generic;
using Godot;

public class LetFly : BruiserCardModel<LetFly.CardTop, LetFly.CardBottom>
{
	public override string Name => "Let Fly";
	public override int Level => 7;
	public override int Initiative => 71;
	protected override int AtlasIndex => 23;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.4637037f, 0.24021162f)))
				.WithTargets(2)
				.WithConditions(Conditions.Poison1)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.50982964f, 0.33842435f)))
				.WithTargets(2)
				.Build())
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}