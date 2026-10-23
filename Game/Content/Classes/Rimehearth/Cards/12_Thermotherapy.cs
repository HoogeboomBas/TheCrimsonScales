using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Thermotherapy : RimehearthCardModel<Thermotherapy.CardTop, Thermotherapy.CardBottom>
{
	public override string Name => "Thermotherapy";
	public override int Level => 1;
	public override int Initiative => 66;
	protected override int AtlasIndex => 12;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.23102495f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(
						[CardElementConsumption.Consume([Element.Fire]), CardElementConsumption.Consume(Element.Ice)],
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAddCondition(Conditions.Brittle);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Brittle))}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.49425563f, 0.6127424f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build(),
								ConditionAbility.Builder().WithConditions(Conditions.Wound1).WithTarget(Target.Self).WithMandatory(true).Build()
							]);
							await actionState.Perform();

							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}