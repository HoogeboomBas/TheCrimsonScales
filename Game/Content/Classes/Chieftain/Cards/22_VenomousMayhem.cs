using System.Collections.Generic;

public class VenomousMayhem : ChieftainCardModel<VenomousMayhem.CardTop, VenomousMayhem.CardBottom>
{
	public override string Name => "Venomous Mayhem";
	public override int Level => 6;
	public override int Initiative => 92;
	protected override int AtlasIndex => 22;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 5,
					Move = 4,
					Attack = 1,
					Traits = 
					[
						new ApplyConditionTrait(Conditions.Poison1),
						new ApplyConditionTrait(Conditions.Immobilize)
					]
				})
				.WithName("Cottonmouth Snake")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/cottonmouth_snake_AI.png")
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(3).WithConditions(Conditions.Poison1).Build()),
		];
	}
}