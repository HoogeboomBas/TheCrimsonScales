using System.Collections.Generic;

public class SoulStrike : HierophantCardModel<SoulStrike.CardTop, SoulStrike.CardBottom>
{
	public override string Name => "Soul Strike";
	public override int Level => 1;
	public override int Initiative => 69;
	protected override int AtlasIndex => 29 - 8;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(4, range: 3, pierce: 3, conditions: [Conditions.Wound1]))
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3)),

			new AbilityCardAbility(new HealAbility(1, target: Target.Allies | Target.TargetAll,
				customGetTargets: (state, list) =>
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
			))
		];
	}
}