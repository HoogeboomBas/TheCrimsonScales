using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BlazingPincers : LuminaryCardModel<BlazingPincers.CardTop, BlazingPincers.CardBottom>
{
	public override string Name => "Blazing Pincers";
	public override int Level => 9;
	public override int Initiative => 59;
	protected override int AtlasIndex => 28;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red, "Wound",
							Icons.GetCondition(Conditions.Wound1)),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast).Add(Direction.East), AOEHexType.Red, "Wound",
							Icons.GetCondition(Conditions.Wound1)),
					]
				))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.GetCustomMarkedHexes("Wound").Contains(parameters.AbilityState.Target.Hex),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			Scuttle(2, global::Elements.All),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
	}
}