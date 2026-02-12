using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class AgilePredator : ChieftainCardModel<AgilePredator.CardTop, AgilePredator.CardBottom>
{
	public override string Name => "Agile Predator";
	public override int Level => 3;
	public override int Initiative => 90;
	protected override int AtlasIndex => 16;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 5,
					Move = 3,
					Attack = 1,
					Traits = 
					[
						new RetaliateTrait(1),
						new AttackersGainDisadvantageTrait(),
						new MountTrait(),
					]
				})
				.WithName("Black Panther")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/black_panther_AI.png")
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
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(1)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons
						.Where(summon => RangeHelper.Distance(grantState.Performer.Hex, summon.Hex) <= 3));
				})
				.WithTarget(Target.Allies)
				.Build()
			),
		];
	}
}