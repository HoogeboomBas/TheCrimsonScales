using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ImposingBrilliance : LuminaryCardModel<ImposingBrilliance.CardTop, ImposingBrilliance.CardBottom>
{
	public override string Name => "Imposing Brilliance";
	public override int Level => 6;
	public override int Initiative => 86;
	protected override int AtlasIndex => 23;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.46372387f, 0.13845979f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red, "Poison", Icons.GetCondition(Conditions.Poison1)),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red, "Poison", Icons.GetCondition(Conditions.Poison1)),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red, "Immobilize",
							Icons.GetCondition(Conditions.Immobilize)),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Empty),
					]
				))
				.WithAfterTargetConfirmedSubscriptions(
				[
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.GetCustomMarkedHexes("Poison").Contains(parameters.AbilityState.Target.Hex),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison1);

							await GDTask.CompletedTask;
						}
					),
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.GetCustomMarkedHexes("Immobilize").Contains(parameters.AbilityState.Target.Hex),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);

							await GDTask.CompletedTask;
						}
					)
				])
				.Build()),
			Scuttle(1, [Element.Dark]),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214329f, 0.65278393f)))
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => figure.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.InfuseElement(state, Element.Fire);
					}

					if(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => figure.AlliedWith(state.Performer)))
					{
						await AbilityCmd.InfuseElement(state, Element.Ice);
					}

					if(state.Origin == state.Performer.Hex)
					{
						await AbilityCmd.InfuseElement(state, Element.Light);
					}

					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}
}