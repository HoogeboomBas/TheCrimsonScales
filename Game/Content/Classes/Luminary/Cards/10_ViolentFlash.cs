using System.Collections.Generic;
using Godot;

public class ViolentFlash : LuminaryCardModel<ViolentFlash.CardTop, ViolentFlash.CardBottom>
{
	public override string Name => "Violent Flash";
	public override int Level => 1;
	public override int Initiative => 40;
	protected override int AtlasIndex => 10;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44441268f, 0.22417206f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
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
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62128437f, 0.7193808f)))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),
		];
	}
}