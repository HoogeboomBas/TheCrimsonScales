using System.Collections.Generic;
using Godot;

public class SoulStrike : HierophantCardModel<SoulStrike.CardTop, SoulStrike.CardBottom>
{
	public override string Name => "Soul Strike";
	public override int Level => 1;
	public override int Initiative => 69;
	protected override int AtlasIndex => 13 - 7;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.3276776f, 0.29315552f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.5516777f, 0.29315552f)))
				.WithPierce(3)
				.WithConditions(Conditions.Wound1)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
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