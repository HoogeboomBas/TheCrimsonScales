using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BreakneckBlitz : TempestCardModel<BreakneckBlitz.CardTop, BreakneckBlitz.CardBottom>
{
	public override string Name => "Breakneck Blitz";
	public override int Level => 1;
	public override int Initiative => 04;
	protected override int AtlasIndex => 5;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscription(
					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.6237831f, 0.333518f)))
				.WithDuringMovementSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringMovement.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AdjustMoveValue(1);
						parameters.AbilityState.AdjustMoveType(MoveType.Jump);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")),

					LoseSparksSubscription<ScenarioEvents.DuringMovement.Parameters>(1, async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}"))
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6185266f, 0.68088645f)))
				.WithDuringMovementSubscriptions(
				[
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}"))
				])
				.WithOnAbilityStarted(async state =>
				{
					List<Hex> hexesWithSparks = [];
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => parameters.PotentialAbilityState == state && !hexesWithSparks.Contains(parameters.Hex) &&
						              parameters.Hex.HasHexObjectOfType<TempestSpark>(),
						async parameters =>
						{
							hexesWithSparks.Add(parameters.Hex);
							state.SetCustomValue(this, "HexesWithSpark", hexesWithSparks.Count);
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
					if(state.GetCustomValue<int>(this, "HexesWithSpark") >= 2)
					{
						await AbilityCmd.AddCondition(state, state.Performer, Conditions.Dodge);
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build())
		];
	}
}