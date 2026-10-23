using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SnapFreeze : RimehearthCardModel<SnapFreeze.CardTop, SnapFreeze.CardBottom>
{
	public override string Name => "Snap Freeze";
	public override int Level => 6;
	public override int Initiative => 08;
	protected override int AtlasIndex => 21;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6169745f, 0.19224377f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Chill);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Chill))}")),
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer.HasWound(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);

							await GDTask.CompletedTask;
						}, effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Wound1)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} from self to {Icons.Inline(Icons.Targets)}1 enemy within 3 hexes")
					)
				])
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.WithOnAbilityEndedPerformed(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => !state.Performer.HasCondition(Conditions.Chill),
						async _ =>
						{
							ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
							await state.ActionState.RequestDiscardOrLose();
						});

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
		public override int XP => 1;
		public override bool Persistent => true;
	}
}