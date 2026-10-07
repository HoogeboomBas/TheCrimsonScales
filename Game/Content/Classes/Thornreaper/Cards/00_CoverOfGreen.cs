using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class CoverOfGreen : ThornreaperCardModel<CoverOfGreen.CardTop, CoverOfGreen.CardBottom>
{
	public override string Name => "Cover of Green";
	public override int Level => 1;
	public override int Initiative => 34;
	protected override int AtlasIndex => 29 - 0;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.234234f, 0.5345345f)))
				.WithConditions(Conditions.Muddle)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), this, new Vector2(0.2f, 0.3f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.SouthWest), this, new Vector2(0.5f, 0.4f))
				)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
				.Build()
				)
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(2, new MoveSquare(this, new Vector2(0.3f, 0.5f)))
			.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
			.WithShieldValue(1)
			.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(state => AbilityCmd.InfuseElement(state, Element.Earth))
			.Build()
			)
		];
		public override bool Round => true;
	}
}