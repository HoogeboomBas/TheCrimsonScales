using System.Collections.Generic;
using Godot;

public class ChestThumper : ChieftainCardModel<ChestThumper.CardTop, ChestThumper.CardBottom>
{
	public override string Name => "Chest Thumper";
	public override int Level => 5;
	public override int Initiative => 94;
	protected override int AtlasIndex => 19;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Lowland Gorilla")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/lowland_gorilla_AI.png")
				.WithHealth(7, new SummonHealthSquare(this, new Vector2(0.4473646f, 0.18335739f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.67825043f, 0.18335739f)))
				.WithAttack(2, new SummonAttackSquare(this, new Vector2(0.4473646f, 0.25967413f)))
				.WithTraits(
					new JumpTrait(),
					new HealOnKillTrait(2),
					new MountTrait()
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithRange(2, new RangeSquare(this, new Vector2(0.7236573f, 0.7644118f)))
				.Build()
			),
		];
	}
}