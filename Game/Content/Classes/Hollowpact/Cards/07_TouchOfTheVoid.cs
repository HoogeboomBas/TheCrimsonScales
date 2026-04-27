using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TouchOfTheVoid : HollowpactCardModel<TouchOfTheVoid.CardTop, TouchOfTheVoid.CardBottom>
{
	public override string Name => "Touch of TheVoid";
	public override int Level => 1;
	public override int Initiative => 29;
	protected override int AtlasIndex => 7;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbility.Builder().Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Stun)
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Dark);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.GetElement(Element.Dark))}")))
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Target == state.Performer && 
						                      canApplyParameters.ConditionModel == Conditions.Muddle,
						async applyParameters =>
						{
							if(!applyParameters.Prevented)
							{
								applyParameters.SetPrevented(true);
							}

							await GDTask.CompletedTask;
						});
					
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					await GainVoidEnergy(state);
					await state.AdvanceUseSlot();
					
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
							async parameters =>
							{
								await GainVoidEnergy(state);
								await state.AdvanceUseSlot();
							});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.17400035f, 0.81451213f)),
					new UseSlot(new Vector2(0.38299766f, 0.81451213f)),
					new UseSlot(new Vector2(0.59050035f, 0.81451213f), GainXP),
					new UseSlot(new Vector2(0.8010102f, 0.81451213f))
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}