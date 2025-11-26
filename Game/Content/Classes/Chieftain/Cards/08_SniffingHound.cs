using System.Collections.Generic;

public class SniffingHound : ChieftainCardModel<SniffingHound.CardTop, SniffingHound.CardBottom>
{
	public override string Name => "Sniffing Hound";
	public override int Level => 1;
	public override int Initiative => 80;
	protected override int AtlasIndex => 8;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 4,
					Move = 3,
					Attack = 1,
					Traits = 
					[
						new TrapDisarmTrait(1)
					]
				})
				.WithName("Scout Dog")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/scout_dog_AI.png")
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
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(5).Build()),
		];
	}
}