using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CellRegeneration : BrightsparkCardModel<CellRegeneration.CardTop, CellRegeneration.CardBottom>
{
	public override string Name => "Cell Regeneration";
	public override int Level => 1;
	public override int Initiative => 73;
	protected override int AtlasIndex => 2;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.49459255f, 0.23310421f), EnhancementCostType.MultiTarget))
				.WithTarget(Target.Self)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							parameters.AbilityState.SetTarget(Target.SelfOrAllies | Target.SelfCountsForTargets);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"{Icons.Inline(Icons.Targets)}1 adjacent ally and self instead")))
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build(),
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29150033f, 0.7689984f)),
					new UseSlot(new Vector2(0.5004996f, 0.7689984f)),
					new UseSlot(new Vector2(0.7074981f, 0.7689984f), FinalSlotAbility)
				])
				.Build())
		];

		public override bool Persistent => true;

		private async GDTask FinalSlotAbility(AbilityState abilityState)
		{
			ActionState actionState = new ActionState(abilityState.Performer,
			[
				HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build(),
			]);
			await actionState.Perform();
			await AbilityCmd.GainXP(abilityState.Performer, 1);
		}
	}
}