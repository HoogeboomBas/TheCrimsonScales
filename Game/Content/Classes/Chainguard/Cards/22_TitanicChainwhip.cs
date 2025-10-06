using System.Collections.Generic;

public class TitanicChainwhip : ChainguardLevelUpCardModel<TitanicChainwhip.CardTop, TitanicChainwhip.CardBottom>
{
	public override string Name => "Titanic Chainwhip";
	public override int Level => 6;
	public override int Initiative => 29;
	protected override int AtlasIndex => 15 - 9;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(3)
				.WithConditions(Chainguard.Shackle)
				.WithPull(2)
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(4)
				.WithRange(5)
				.WithConditions(Chainguard.Shackle, Conditions.Wound1)
				.Build()),
		];  
	}
}