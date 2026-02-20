using System.Collections.Generic;
using Godot;

public class BloodThinner : MirefootCardModel<BloodThinner.CardTop, BloodThinner.CardBottom>
{
	public override string Name => "Blood Thinner";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 0;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.49809915f, 0.2793195f)))
				.WithConditions(Conditions.Wound2)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Wound1, Conditions.Immobilize])
				.WithRange(2, new RangeSquare(this, new Vector2(0.65683514f, 0.7661615f)))
				.Build())
		];
	}
}