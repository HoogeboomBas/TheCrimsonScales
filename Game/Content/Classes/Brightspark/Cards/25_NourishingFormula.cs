using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class NourishingFormula : BrightsparkCardModel<NourishingFormula.CardTop, NourishingFormula.CardBottom>
{
	public override string Name => "Nourishing Formula";
	public override int Level => 7;
	public override int Initiative => 77;
	protected override int AtlasIndex => 25;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.25777778f, 0.13968253f)))
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithTargets(2)
				.WithRange(3)
				.WithDuringHealSubscriptions(
					[
						ScenarioEvents.DuringHeal.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAdjustHealValue(1);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}")
						),
						ScenarioEvents.DuringHeal.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AbilityAdjustHealValue(1);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}")
						)
					]
				)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithOnAbilityEndedPerformed(async attackAbilityState =>
						{
							await AbilityCmd.InfuseWildElement(state);
							await AbilityCmd.GainXP(state.Performer, 1);
						}).Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<HealAbility.State>(0).UniqueTargetedFigures
						.Where(figure => figure != state.Performer));
					GD.Print(figures.Count);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					return state.Performer is Character character &&
					       character.TurnItemsUsed.Any(item => item.ItemState == ItemState.Consumed && item.ItemType == ItemType.Small) &&
					       await AbilityCmd.HasPerformedAbility(state, 0);
				})
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62222224f, 0.6190476f)))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && state.UseSlotIndex <= 2,
						async parameters =>
						{
							Ability ability = state.UseSlotIndex switch
							{
								0 => MoveAbility.Builder().WithDistance(3).Build(),
								1 => AttackAbility.Builder().WithDamage(3).Build(),
								2 => ShieldAbility.Builder().WithShieldValue(1).Build(),
								_ => throw new ArgumentOutOfRangeException()
							};
							await new ActionState(state.Performer, [ability]).Perform();
							if(state.UseSlotIndex < 2)
							{
								await state.AdvanceUseSlot();
							}
							else
							{
								ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
									roundEndedParameters => true,
									async roundEndedParameters =>
									{
										await state.AdvanceUseSlot();
									});
							}
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29037037f, 0.87671953f)),
					new UseSlot(new Vector2(0.49777776f, 0.87671953f)),
					new UseSlot(new Vector2(0.70666665f, 0.87671953f))
				])
				.Build())
		];

		public override bool Persistent => true;
	}
}