using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SolarFlare : ThornreaperLevelUpCardModel<SolarFlare.CardTop, SolarFlare.CardBottom>
{
	public override string Name => "Solar Flare";
	public override int Level => 6;
	public override int Initiative => 36;
	protected override int AtlasIndex => 22;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(4, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				]), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), this, new Vector2(0.2f, 0.3f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
		[
			CardElementInfusion.Infuse(Element.Fire),
			CardElementInfusion.Infuse(Element.Light)
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if (GetLightElementState() is ElementState.Strong or ElementState.Waning)
					{
						await AbilityCmd.InfuseElement(state, Element.Light);
						await AbilityCmd.InfuseElement(state, Element.Earth);
					}
					state.SetPerformed();
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];
	}
}
