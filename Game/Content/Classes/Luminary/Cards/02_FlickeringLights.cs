using System.Collections.Generic;
using Godot;

public class FlickeringLights : LuminaryCardModel<FlickeringLights.CardTop, FlickeringLights.CardBottom>
{
	public override string Name => "Flickering Lights";
	public override int Level => 1;
	public override int Initiative => 19;
	protected override int AtlasIndex => 2;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4579801f, 0.14994247f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Empty),
					]
				))
				.WithOnAbilityEndedPerformed(async state =>
				{
					for(int i = 0; i < state.UniqueTargetedFigures.Count; i++)
					{
						await AbilityCmd.InfuseWildElement(state);
					}
				})
				.Build()),
			Scuttle(1, [Element.Light]),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.LootedObjects.Count >= 2)
					{
						await AbilityCmd.InfuseWildElement(state);
					}
				})
				.Build())
		];
	}
}