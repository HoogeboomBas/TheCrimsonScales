using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AirborneSpores : MirefootCardModel<AirborneSpores.CardTop, AirborneSpores.CardBottom>
{
	public override string Name => "Airborne Spores";
	public override int Level => 1;
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
						parameters => parameters.AbilityState.Target.EnemiesWith(parameters.Performer) && 
							RangeHelper.Distance(parameters.Performer.Hex, parameters.AbilityState.Target.Hex) == 1,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetRemoveCondition(Conditions.Poison1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison2);

							await GDTask.CompletedTask;
						}
					)
				)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithCustomGetTargets((state, figures) =>
				{
					ConditionAbility.State conditionAbilityState = state.ActionState.GetAbilityState<ConditionAbility.State>(0);

					foreach((Vector2I coords, AOEHexType hexType) in conditionAbilityState.AOEHexes)
					{
						if(hexType == AOEHexType.Red)
						{
							Hex hex = GameController.Instance.Map.GetHex(coords);
							if(hex != null)
							{
								figures.AddRange(hex.GetHexObjectsOfType<Figure>().Where(figure => figure.AlliedWith(state.Performer)));
							}
						}
					}
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithMandatory(true)
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
				.WithTarget(Target.Enemies | Target.TargetAll)
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