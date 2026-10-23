using System.Collections.Generic;
using System.Linq;

public class GlacialCocoon : RimehearthCardModel<GlacialCocoon.CardTop, GlacialCocoon.CardBottom>
{
	public override string Name => "Glacial Cocoon";
	public override int Level => 1;
	public override int Initiative => 14;
	protected override int AtlasIndex => 11;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Chill, Conditions.Chill])
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override bool Round => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6)
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.Hexes
						        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
						        .Where(figure => state.Performer.EnemiesWith(figure))
						        .Distinct())
					{
						await AbilityCmd.AddCondition(state, figure, Conditions.Chill);
					}
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}