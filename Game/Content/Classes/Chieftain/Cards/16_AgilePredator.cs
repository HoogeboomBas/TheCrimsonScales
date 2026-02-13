using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AgilePredator : ChieftainCardModel<AgilePredator.CardTop, AgilePredator.CardBottom>
{
	public override string Name => "Agile Predator";
	public override int Level => 3;
	public override int Initiative => 90;
	protected override int AtlasIndex => 16;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Black Panther")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/black_panther_AI.png")
				.WithHealth(5, new SummonHealthSquare(this, new Vector2(0.44668853f, 0.20578967f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67717457f, 0.20578967f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.44668853f, 0.2806894f)))
				.WithTraits(
					new RetaliateTrait(1),
					new AttackersGainDisadvantageTrait(),
					new MountTrait()
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					AbilityCmd.SummonMovePlusX(1).Build()
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