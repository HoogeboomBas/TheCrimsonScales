using System.Collections.Generic;

public class MoundedSight : ChieftainCardModel<MoundedSight.CardTop, MoundedSight.CardBottom>
{
	public override string Name => "Mounded Sight";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 12;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 5,
					Move = 2,
					Attack = 1,
					Traits = 
					[
						new IgnoreDifficultAndHazardousTerrainTrait(),
						new MountTrait(),
					]
				})
				.WithName("Cavalry Camel")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/cavalry_camel_AI.png")
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
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(PushAbility.Builder().WithPush(2).Build()),
		];
	}
}