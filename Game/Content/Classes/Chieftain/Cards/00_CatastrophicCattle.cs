using System.Collections.Generic;

public class CatastrophicCattle : ChieftainCardModel<CatastrophicCattle.CardTop, CatastrophicCattle.CardBottom>
{
	public override string Name => "Catastrophic Cattle";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 0;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 4,
					Move = 2,
					Attack = 2,
					Traits = 
					[
						new MountTrait(),
						new PushTrait(1),
					]
				})
				.WithName("Fighting Bull")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/fighting_bull_AI.png")
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
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
	}
}