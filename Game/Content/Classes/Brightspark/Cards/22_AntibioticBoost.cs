using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AntibioticBoost : BrightsparkCardModel<AntibioticBoost.CardTop, AntibioticBoost.CardBottom>
{
	public override string Name => "Antibiotic Boost";
	public override int Level => 6;
	public override int Initiative => 56;
	protected override int AtlasIndex => 22;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bless)
				.WithTarget(Target.TargetAll | Target.SelfOrAllies)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3)
						        .Where(figure => figure.AlliedWith(state.Performer, true)))
					{
						await AbilityCmd.RemoveAllNegativeConditions(figure);
						state.SetPerformed();
					}
				})
				.Build())
		];

		public override int XP => 1;
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
							await state.AdvanceUseSlot();
							if(state.GetCustomValue<bool>(this, "ElementsConsumed"))
							{
								await state.AdvanceUseSlot();
								await state.AdvanceUseSlot();
							}
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
					new UseSlot(new Vector2(0.29185185f, 0.8783068f), FirstSlotAbility),
					new UseSlot(new Vector2(0.49925926f, 0.8783068f), SecondSlotAbility),
					new UseSlot(new Vector2(0.70666665f, 0.8783068f), ThirdSlotAbility)
				])
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(
						[CardElementConsumption.Consume(Element.Air), CardElementConsumption.Consume(Element.Light)],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "ElementsConsumed", true);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform all abilities at the end of this turn instead.")))
				.Build())
		];

		public override bool Persistent => true;

		private async GDTask FirstSlotAbility(AbilityState abilityState)
		{
			ActionState actionState = new ActionState(abilityState.Performer,
			[
				ConditionAbility.Builder().WithConditions(Conditions.Strengthen).WithTarget(Target.Self).Build(),
			]);
			await actionState.Perform();
		}

		private async GDTask SecondSlotAbility(AbilityState abilityState)
		{
			ActionState actionState = new ActionState(abilityState.Performer,
			[
				HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build(),
			]);
			await actionState.Perform();
		}

		private async GDTask ThirdSlotAbility(AbilityState abilityState)
		{
			ActionState actionState = new ActionState(abilityState.Performer,
			[
				ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Self).Build(),
			]);
			await actionState.Perform();
			await AbilityCmd.GainXP(abilityState.Performer, 1);
		}
	}
}