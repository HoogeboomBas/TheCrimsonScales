using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ColdSnap : RimehearthCardModel<ColdSnap.CardTop, ColdSnap.CardBottom>
{
	public override string Name => "Cold Snap";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 3;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.44401118f, 0.23082495f)))
				.WithAdvantage()
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Chill, Conditions.Chill])
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Chill, Conditions.Chill])
				.WithRange(2)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
	}
}