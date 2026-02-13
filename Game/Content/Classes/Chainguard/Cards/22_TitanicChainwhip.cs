using System.Collections.Generic;
using Godot;

public class TitanicChainwhip : ChainguardLevelUpCardModel<TitanicChainwhip.CardTop, TitanicChainwhip.CardBottom>
{
	public override string Name => "Titanic Chainwhip";
	public override int Level => 6;
	public override int Initiative => 29;
	protected override int AtlasIndex => 15 - 9;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.32713646f, 0.29301867f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.55225706f, 0.29301867f)))
				.WithConditions(Chainguard.Shackle)
				.WithPull(2)
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(4)
				.WithRange(5)
				.WithConditions([Chainguard.Shackle, Conditions.Wound1])
				.Build()),
		];
	}
}