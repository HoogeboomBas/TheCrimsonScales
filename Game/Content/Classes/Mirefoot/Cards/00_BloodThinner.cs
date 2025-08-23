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
			new AbilityCardAbility(new AttackAbility(1, conditions: [Conditions.Wound2]))
		];

		protected override int XP => 1;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(conditions: [Conditions.Wound1, Conditions.Immobilize])
				.WithRange(2)
				.Build())
		];
	}
}