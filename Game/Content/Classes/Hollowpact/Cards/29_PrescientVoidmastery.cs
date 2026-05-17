using System.Collections.Generic;
using System.Linq;
using Godot;

public class PrescientVoidmastery : HollowpactLevelUpCardModel<PrescientVoidmastery.CardTop, PrescientVoidmastery.CardBottom>
{
	public override string Name => "Prescient Voidmastery";
	public override int Level => 9;
	public override int Initiative => 11;
	protected override int AtlasIndex => 15;

	public class CardTop : HollowpactCardSide
	{
		private AttackEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new AttackDiamond(this, new Vector2(0.41027772f, 0.26666665f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async otherState =>
				{
					ScenarioEvents.AbilityEndedEvent.Subscribe(otherState, this, 
						abilityEndedParameters => abilityEndedParameters.Performer.EnemiesWith(otherState.Performer) && 
							abilityEndedParameters.AbilityState is MoveAbility.State or TeleportAbility.State &&
							RangeHelper.Distance(abilityEndedParameters.Performer.Hex, otherState.Performer.Hex) == 1,
						async abilityEndedParameters =>
						{
							ActionState actionState = new ActionState(otherState.ActionState, otherState.Performer,
							[
								AttackAbility.Builder()
									.WithDamage(3, _enhancementMark)
									.WithDuringAttackSubscriptions([
										LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
											async parameters =>
											{
												parameters.AbilityState.AbilityAdjustAttackValue(2);
											},
											new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}")),

										ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.Consume(Element.Dark)],
											applyFunction: async parameters =>
											{
												parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);
											},
											effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Disarm))}")
										)
									])
									.WithCustomGetTargets((attackState, figures) =>
									{
										figures.Add(abilityEndedParameters.Performer);
									})
									.Build()
							]);

							await actionState.Perform();
						});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, this);
				})
				.Build()),
		];

		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6183333f, 0.6226336f)))
				.WithOnAbilityEndedPerformed(async state =>
				{
					IEnumerable<Figure> figures = state.Hexes
						.SelectMany(hex => hex.Neighbours)
						.Distinct()
						.SelectMany(hex => hex.GetFigures())
						.Except([state.Performer]);
					
					foreach(Figure figure in figures)
					{
						await AbilityCmd.SufferDamage(state, figure, 1, state.Performer);
						await AbilityCmd.AddCondition(state, figure, Conditions.Wound1);
					}
				})
				.Build()),

			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(3)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder(3)
				.Build()),
		];
	}
}