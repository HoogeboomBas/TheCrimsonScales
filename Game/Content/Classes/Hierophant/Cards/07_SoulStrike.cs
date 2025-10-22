using System.Collections.Generic;

public class SoulStrike : HierophantCardModel<SoulStrike.CardTop, SoulStrike.CardBottom>
{
	public override string Name => "Soul Strike";
	public override int Level => 1;
	public override int Initiative => 69;
	protected override int AtlasIndex => 13 - 7;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(3)
				.WithPierce(3)
				.WithConditions(Conditions.Wound1)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((state, list) =>
					{
						MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

						foreach(Hex hex in moveAbilityState.Hexes)
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
							{
								if(state.Performer.AlliedWith(figure))
								{
									list.Add(figure);
								}
							}
						}
					}
				)
				.Build())
		];
	}
}