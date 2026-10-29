using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RushingWind : TempestCardModel<RushingWind.CardTop, RushingWind.CardBottom>
{
	public override string Name => "Rushing Wind";
	public override int Level => 7;
	public override int Initiative => 64;
	protected override int AtlasIndex => 24;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210548f, 0.1296399f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6195546f, 0.20034003f)))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")),

					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(parameters.AbilityState.SingleTargetAttackValue);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters($"Double the value of the attack, {Icons.Inline(Icons.LoseCard)}"))
				])
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214549f, 0.62060654f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					int value = 1 + +state.GetCustomValue<int>(this, "AirConsumed");
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(value);
							parameters.AbilityState.SingleTargetAdjustPush(value);

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithAbilityStartedSubscription(
					LoseSparksConsumeElementSubscription<ScenarioEvents.AbilityStarted.Parameters>(1, Element.Air, applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "AirConsumed", 1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Add +2{Icons.Inline(Icons.Attack)} and +2{Icons.Inline(Icons.Push)} instead")))
				.Build())
		];

		public override bool Round => true;
	}
}