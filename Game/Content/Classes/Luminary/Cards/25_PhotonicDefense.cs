using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PhotonicDefense : LuminaryCardModel<PhotonicDefense.CardTop, PhotonicDefense.CardBottom>
{
	public override string Name => "Photonic Defense";
	public override int Level => 7;
	public override int Initiative => 09;
	protected override int AtlasIndex => 25;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApply: parameters => state.Performer == parameters.AbilityState.Target,
						apply: async parameters =>
						{
							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription shieldChosenSubscription =
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async applyParameters =>
									{
										ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
											canApplyParameters => canApplyParameters.Figure == state.Performer && canApplyParameters.FromAttack,
											async applyParameters =>
											{
												applyParameters.AdjustShield(3);
												ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

												await GDTask.CompletedTask;
											}
										);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Shield),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"Gain {Icons.Inline(Icons.Shield)}3 for the attack"),
									effectType: EffectType.SelectableMandatory
								);

							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription retaliateChosenSubscription =
								ScenarioEvents.GenericChoice.Subscription.New(
									applyFunction: async applyParameters =>
									{
										ScenarioEvents.RetaliateEvent.Subscribe(state, this,
											canApplyParameters =>
											{
												return canApplyParameters.RetaliatingFigure == state.Performer &&
												       RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1;
											},
											async applyParameters =>
											{
												applyParameters.AdjustRetaliate(3);
												ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

												await GDTask.CompletedTask;
											}
										);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Retaliate),
									effectInfoViewParameters: new TextEffectInfoView.Parameters(
										$"Gain {Icons.Inline(Icons.Retaliate)}3 for the attack"),
									effectType: EffectType.SelectableMandatory
								);

							await AbilityCmd.GenericChoice(state.Performer,
								[shieldChosenSubscription, retaliateChosenSubscription],
								hintText: "Select an effect to gain for the attack:");

							switch(state.UseSlotIndex)
							{
								case 0:
									await AbilityCmd.InfuseElement(state, Element.Fire);
									break;
								case 1:
									await AbilityCmd.InfuseElement(state, Element.Ice);
									break;
								case 2:
									await AbilityCmd.InfuseElement(state, Element.Light);
									break;
								case 3:
									await AbilityCmd.InfuseElement(state, Element.Dark);
									break;
							}

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.15999386f, 0.3559997f)),
					new UseSlot(new Vector2(0.36899313f, 0.3559997f)),
					new UseSlot(new Vector2(0.5764929f, 0.3559997f)),
					new UseSlot(new Vector2(0.7889926f, 0.3559997f), Gain2XP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6214329f, 0.63088423f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Performer == state.Performer &&
						                        parameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability"),
						apply: async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [
								MoveAbility.Builder()
									.WithDistance(3)
									.Build()
							]);
							await actionState.Perform();
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override bool Round => true;
	}
}