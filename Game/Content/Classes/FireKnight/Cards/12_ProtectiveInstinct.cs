using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ProtectiveInstinct : FireKnightCardModel<ProtectiveInstinct.CardTop, ProtectiveInstinct.CardBottom>
{
	public override string Name => "Protective Instinct";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 12 - 12;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					ShieldAbility.Builder()
						.WithShieldValue(1, new ShieldSquare(this, new Vector2(0.61310434f, 0.36479843f)))
						.Build()
				])
				.WithTarget(Target.SelfOrAllies)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.617708f, 0.6765259f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(3, new MoveSquare(this, new Vector2(0.617708f, 0.8635235f)))
						.Build()
				])
				.WithRange(2)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						return state.Performer.Hex.HasHexObjectOfType<Ladder>();
					}
				)
				.Build()),
		];
	}
}