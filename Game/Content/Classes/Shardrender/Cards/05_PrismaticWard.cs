using System.Collections.Generic;
using Godot;

public class PrismaticWard : ShardrenderCardModel<PrismaticWard.CardTop, PrismaticWard.CardBottom>
{
	public override string Name => "Prismatic Ward";
	public override int Level => 1;
	public override int Initiative => 54;
	protected override int AtlasIndex => 5;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.39734706f, 0.3628809f)),
						new UseSlot(new Vector2(0.60455734f, 0.3628809f))
					]
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6215549f, 0.7162435f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward)
				.WithTarget(Target.Self)
				.Build())
		];
	}
}