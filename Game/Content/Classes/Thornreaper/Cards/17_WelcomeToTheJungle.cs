using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WelcomeToTheJungle : ThornreaperLevelUpCardModel<WelcomeToTheJungle.CardTop, WelcomeToTheJungle.CardBottom>
{
	public override string Name => "Welcome to the Jungle";
	public override int Level => 3;
	public override int Initiative => 41;
	protected override int AtlasIndex => 17;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPierce(2)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
				]),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East), this, new Vector2(0.2f, 0.3f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast), this, new Vector2(0.2f, 0.3f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East), this, new Vector2(0.2f, 0.3f)))
				.WithConditionalAbilityCheck(abilityState => AbilityCmd.HasPerformedAbility(abilityState, 0))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithOnAbilityEndedPerformed(async pullAbilityState =>
				{
					if (pullAbilityState.Target?.Hex?.HasHexObjectOfType<HazardousTerrain>() == true)
					{
						await AbilityCmd.AddCondition(pullAbilityState, pullAbilityState.Performer, Conditions.Strengthen);
						await AbilityCmd.GainXP(pullAbilityState.Performer, 1);
					}
				})
				.Build())
		];
	}
}
