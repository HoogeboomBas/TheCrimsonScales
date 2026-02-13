using System.Collections.Generic;
using Godot;

public class RustySpikes : ChainguardCardModel<RustySpikes.CardTop, RustySpikes.CardBottom>
{
	public override string Name => "Rusty Spikes";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 12 - 1;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Poison1)
				.WithCustomAsset("res://Content/Classes/Chainguard/Traps/ChainguardPoisonTrap.tscn")
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6204333f, 0.7188443f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),
		];
	}
}