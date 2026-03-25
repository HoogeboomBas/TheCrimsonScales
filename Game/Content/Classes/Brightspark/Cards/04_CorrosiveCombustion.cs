using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CorrosiveCombustion : BrightsparkCardModel<CorrosiveCombustion.CardTop, CorrosiveCombustion.CardBottom>
{
	public override string Name => "Corrosive Combustion";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 4;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetAOEPattern(new AOEPattern(
								[
									new AOEHex(Vector2I.Zero, AOEHexType.Gray),
									new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
									new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
								]
							));

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Increase the area of effect as shown")
					)
				)
				.Build()),
		];

		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.AbilityState.SingleTargetRangeType == RangeType.Range,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(1);
							await GDTask.CompletedTask;
						});
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.AbilityState.SingleTargetRangeType == RangeType.Range,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPierce(2);
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29100034f, 0.76699734f)),
					new UseSlot(new Vector2(0.4990001f, 0.76699734f), state => AbilityCmd.InfuseElement(state, Element.Fire)),
					new UseSlot(new Vector2(0.7075f, 0.76699734f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
	}
}