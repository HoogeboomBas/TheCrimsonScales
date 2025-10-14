using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class OaksEmbrace : HierophantCardModel<OaksEmbrace.CardTop, OaksEmbrace.CardBottom>
{
	public override string Name => "Oaks Embrace";
	public override int Level => 1;
	public override int Initiative => 84;
	protected override int AtlasIndex => 13 - 10;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.AlliedWith(canApplyParameters.AbilityState.Target) &&
							RangeHelper.Distance(state.Performer.Hex, canApplyParameters.AbilityState.Target.Hex) <= 3,
						async applyParameters =>
						{
							await AbilityCmd.AddCondition(state, applyParameters.AbilityState.Target, Conditions.Ward);

							await GDTask.DelayFastForwardable(0.3f);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.398f, 0.344f)),
						new UseSlot(new Vector2(0.603f, 0.344f), GainXP)
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
					[
						RetaliateAbility.Builder()
							.WithRetaliateValue(1)
							.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
							.WithOnAbilityEndedPerformed(async state =>
							{
								state.ActionState.SetOverrideRound();

								await GDTask.CompletedTask;
							})
							.Build()
					]
				)
				.Build())
		];
	}
}