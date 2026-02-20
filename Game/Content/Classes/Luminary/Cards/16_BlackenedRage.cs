using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BlackenedRage : LuminaryCardModel<BlackenedRage.CardTop, BlackenedRage.CardBottom>
{
	public override string Name => "Blackened Rage";
	public override int Level => 3;
	public override int Initiative => 45;
	protected override int AtlasIndex => 16;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44796452f, 0.14355949f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")
					)
				)
				.Build()),
			Scuttle(1, [Element.Fire]),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5242182f, 0.705998f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					foreach(Figure figure in moveAbilityState.Hexes
						        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
						        .Where(figure => figure.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.SufferDamage(state, figure, 1);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
				.Build()),
		];
	}
}