using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;
using System.Numerics;
using System;

public class CelestialManeuver : StarslingerCardModel<CelestialManeuver.CardTop, CelestialManeuver.CardBottom>
{
	public override string Name => "Celestial Maneuver";
	public override int Level => 8;
	public override int Initiative => 88;
	protected override int AtlasIndex => 25;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
							[
								GrantAbility.Builder()
									.WithGetAbilities(grantAbilityState =>
									[
										MoveAbility.Builder().WithDistance(2).Build()
									])
									.WithRange(int.MaxValue)
									.Build()
							]);
							await actionState.Perform();
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					MoveAbility.Builder()
						.WithDistance(3)
						.WithDuringMovementSubscription(
							ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Dark,
								applyFunction: async parameters =>
								{
									parameters.AbilityState.AdjustMoveValue(2);

									await GDTask.CompletedTask;
								},
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")
							)
						)
						.Build()
				])
				.WithRange(3)
				.WithTarget(Target.SelfOrAllies)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetCustomValue(this, "ChoseGrant", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform grant ability"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform control ability"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithGetAbilities(controlAbilityState =>
				[
					MoveAbility.Builder()
						.WithDistance(3)
						.WithDuringMovementSubscription(
							ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Dark,
								applyFunction: async parameters =>
								{
									parameters.AbilityState.AdjustMoveValue(2);

									await GDTask.CompletedTask;
								},
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")
							)
						)
						.Build()
				])
				.WithRange(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.ActionState.GetAbilityState<GrantAbility.State>(0).GetCustomValue<bool>(this, "ChoseGrant");
				})
				.Build())
		];
	}
}