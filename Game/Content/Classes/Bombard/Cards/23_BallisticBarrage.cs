using System.Collections.Generic;
using Godot;

public class BallisticBarrage : BombardCardModel<BallisticBarrage.CardTop, BallisticBarrage.CardBottom>
{
	public override string Name => "Ballistic Barrage";
	public override int Level => 7;
	public override int Initiative => 73;
	protected override int AtlasIndex => 23;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithRangeType(RangeType.Range)
						.WithTargetHex(hex)
						.WithTarget(Target.Enemies | Target.TargetAll)
						.WithCustomGetTargets((_, figures) =>
						{
							figures.AddRange(RangeHelper.GetFiguresInRange(hex, 2));
						})
						.Build(),
				])
				.WithAbilityCardSide(this)
				.WithRange(5, new ProjectileRangeSquare(this, new Vector2(0.33703706f, 0.16507936f)))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(5, new AttackDiamond(this, new Vector2(0.44888887f, 0.7915343f)))
						.WithRange(4, new RangeSquare(this, new Vector2(0.65925926f, 0.79123425f)))
						.Build()
				])
				.Build())
		];
	}
}