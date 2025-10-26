using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AirborneSpores : MirefootCardModel<AirborneSpores.CardTop, AirborneSpores.CardBottom>
{
	public override string Name => "Airborne Spores";
	public override int Level => 2;
	public override int Initiative => 21;
	protected override int AtlasIndex => 14;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
					]
				))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.ConditionAfterTargetConfirmed.Subscription.New(
						parameters => RangeHelper.Distance(parameters.Performer.Hex, parameters.AbilityState.Target.Hex) == 1,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetRemoveCondition(Conditions.Poison1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison2);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0)
				.WithConditions(Conditions.Muddle)
				.WithRangeType(RangeType.Range)
				.WithCustomGetTargets((state, list) =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3))
						{
							if(figure.HasPoison())
							{
								list.Add(figure);
							}
						}
					}
				)
				.Build())
		];
	}
}