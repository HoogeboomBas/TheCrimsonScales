using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FrostyGlimmer : LuminaryCardModel<FrostyGlimmer.CardTop, FrostyGlimmer.CardBottom>
{
	public override string Name => "Frosty Glimmer";
	public override int Level => 1;
	public override int Initiative => 55;
	protected override int AtlasIndex => 3;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.6074004f, 0.19765684f)))
				.WithDuringHealSubscriptions(
				[
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(1);
							parameters.AbilityState.AbilityAdjustRange(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}, +1{Icons.Inline(Icons.Range)}")
					),
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")
					)
				])
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6215844f, 0.6384345f)))
				.WithDuringMovementSubscriptions(
				[
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							ScenarioCheckEvents.MoveCheckEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters =>
									canApplyParameters.AbilityState == parameters.AbilityState && canApplyParameters.Hex.HasHexObjectOfType<Trap>(),
								applyParameters =>
								{
									if(applyParameters.Hex.HasHexObjectOfType<Trap>())
									{
										applyParameters.SetAffectedByNegativeHex(false);
									}
								}
							);

							ScenarioCheckEvents.CanPassTrapCheckEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters =>
									canApplyParameters.AbilityState == parameters.AbilityState,
								applyParameters =>
								{
									applyParameters.SetCanPass();
								}
							);

							ScenarioEvents.TrapTriggeredEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == parameters.AbilityState.Performer,
								async applyParameters =>
								{
									applyParameters.SetTriggersTrap(false);
									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.AbilityEndedEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters => canApplyParameters.AbilityState == parameters.AbilityState,
								async applyParameters =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(parameters.AbilityState, this);
									ScenarioEvents.TrapTriggeredEvent.Unsubscribe(parameters.AbilityState, this);
									ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(parameters.AbilityState, this);

									Hex hex = await AbilityCmd.SelectHex(parameters.AbilityState, list =>
									{
										list.AddRange(parameters.AbilityState.Hexes.Where(hex => hex.HasHexObjectOfType<Trap>()));
									}, hintText: "Select a trap to destroy");
									if(hex != null)
									{
										await hex.GetHexObjectOfType<Trap>().Destroy();
									}
								}
							);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Ignore traps, destroy one moved through")
					),
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				])
				.Build())
		];
	}
}