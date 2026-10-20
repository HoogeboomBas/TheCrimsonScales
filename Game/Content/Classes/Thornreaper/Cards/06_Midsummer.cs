using System.Collections.Generic;
using Godot;

public class Midsummer : ThornreaperCardModel<Midsummer.CardTop, Midsummer.CardBottom>
{
	public override string Name => "Midsummer";
	public override int Level => 1;
	public override int Initiative => 50;
	protected override int AtlasIndex => 6;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.3f, 0.5f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						if (GetLightElementState() is ElementState.Waning)
						{
							await AbilityCmd.InfuseElement(state, Element.Light);
						}
					})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build())
		];
	}
}