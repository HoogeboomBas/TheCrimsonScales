using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VersatileConcoction : BrightsparkCardModel<VersatileConcoction.CardTop, VersatileConcoction.CardBottom>
{
	public override string Name => "Versatile Concoction";
	public override int Level => 8;
	public override int Initiative => 58;
	protected override int AtlasIndex => 27;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.617037f, 0.16772486f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Immobilize)))),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAdjustPush(2);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}2"))
				])
				.Build()),
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
							await AbilityCmd.InfuseWildElement(state);
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
					new UseSlot(new Vector2(0.257037f, 0.79999995f), GainXP),
					new UseSlot(new Vector2(0.46222222f, 0.79999995f), async state => await AbilityCmd.InfuseElement(state, Element.Fire)),
					new UseSlot(new Vector2(0.6696296f, 0.79999995f), GainXP),
					new UseSlot(new Vector2(0.35999998f, 0.8994708f), async state => await AbilityCmd.InfuseWildElement(state)),
					new UseSlot(new Vector2(0.5674074f, 0.8994708f), async state =>
					{
						await AbilityCmd.InfuseWildElement(state);
						await AbilityCmd.InfuseWildElement(state);
					})
				])
				.Build()),
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}