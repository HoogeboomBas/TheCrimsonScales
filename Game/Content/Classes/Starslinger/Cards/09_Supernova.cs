using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class Supernova : StarslingerCardModel<Supernova.CardTop, Supernova.CardBottom>
{
	public override string Name => "Supernova";
	public override int Level => 1;
	public override int Initiative => 30;
	protected override int AtlasIndex => 9;

	public class CardTop : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}
					)
				)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							parameters.AbilityState.AbilityAdjustRange(-1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, -1{Icons.Inline(Icons.Range)}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer && !parameters.Performer.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.3649924f, 0.7585101f), GainXP),
					new UseSlot(new Vector2(0.5739941f, 0.7585101f), Light),
					new UseSlot(new Vector2(0.47049105f, 0.8870161f), GainXP)
				])
				.Build())
		];

		protected override bool Persistent => true;
		protected override bool Loss => true;

		private async GDTask Light(AbilityState state)
        {
			await AbilityCmd.InfuseElement(Element.Light);
        }
	}
}