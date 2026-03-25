using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AirborneSkyrockets : BombardCardModel<AirborneSkyrockets.CardTop, AirborneSkyrockets.CardBottom>
{
	public override string Name => "Airborne Skyrockets";
	public override int Level => 7;
	public override int Initiative => 55;
	protected override int AtlasIndex => 22;

	public class CardTop : BombardCardSide
	{
		private AttackEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new AttackDiamond(this, new Vector2(0.5088889f, 0.30135813f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(5, _enhancementMark)
						.WithConditions(Conditions.Immobilize)
						.WithRangeType(RangeType.Range)
						.WithTargetHex(hex)
						.Build(),
				])
				.WithAbilityCardSide(this)
				.WithRange(5, new ProjectileRangeSquare(this, new Vector2(0.33703706f, 0.16507936f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullSelfAbility.Builder()
				.WithPullSelfValue(6)
				.WithRange(7)
				.Build())
		];
	}
}