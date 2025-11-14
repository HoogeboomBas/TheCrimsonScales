using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BerserkBarrage : RuinmawCardModel<BerserkBarrage.CardTop, BerserkBarrage.CardBottom>
{
	public override string Name => "Berserk Barrage";
	public override int Level => 1;
	public override int Initiative => 53;
	protected override int AtlasIndex => 5;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Rupture)
				.WithPierce(1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					]
				))
				.Build()),
		];

		protected override bool Sate => true;
		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return IsSated(state.Performer);
				})
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build())
		];
	}
}