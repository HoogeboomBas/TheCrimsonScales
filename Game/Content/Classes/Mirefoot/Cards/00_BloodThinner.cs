using System.Collections.Generic;

public class BloodThinner : MirefootCardModel<BloodThinner.CardTop, BloodThinner.CardBottom>
{
	public override string Name => "Blood Thinner";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 0;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Wound2)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1, Conditions.Immobilize)
				.WithRange(2)
				.Build())
		];
	}
}