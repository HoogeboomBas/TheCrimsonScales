using System.Collections.Generic;

public class RustySpikes : ChainguardCardModel<RustySpikes.CardTop, RustySpikes.CardBottom>
{
	public override string Name => "Rusty Spikes";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 12 - 1;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Poison1)
				// .WithCustomAsset("cs-poison-trap.png")
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),
		];
	}
}