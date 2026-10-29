using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Cryoseism : RimehearthCardModel<Cryoseism.CardTop, Cryoseism.CardBottom>
{
	public override string Name => "Cryoseism";
	public override int Level => 6;
	public override int Initiative => 77;
	protected override int AtlasIndex => 22;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.SouthEast), AOEHexType.Red),
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), this,
						new Vector2(0.86511636f, 0.24926148f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasWound(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						})
				)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.UniqueTargetedFigures.Any(figure => figure.HasWound()))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async applyParameters =>
						{
							if(applyParameters.Performer.TryGetCondition(Conditions.Chill, out Condition chill))
							{
								applyParameters.AbilityState.AdjustMoveValue(chill.StackCount);
							}

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"This movement is unaffected by {Icons.Inline(Icons.GetCondition(Conditions.Chill))}")
					)
				)
				.WithOnAbilityStarted(async state =>
				{
					if(state.Performer.HasCondition(Conditions.Chill))
					{
						state.AddJump();
					}

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.Performer.HasCondition(Conditions.Chill))
					{
						foreach(Figure figure in state.Hexes
							        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
							        .Where(figure => state.Performer.EnemiesWith(figure))
							        .Distinct())
						{
							await AbilityCmd.AddCondition(state, figure, Conditions.Chill);
						}
					}
				})
				.Build()),
		];
	}
}