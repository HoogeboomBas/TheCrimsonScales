using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TurbulentAbsorption : ShardrenderCardModel<TurbulentAbsorption.CardTop, TurbulentAbsorption.CardBottom>
{
	public override string Name => "Turbulent Absorption";
	public override int Level => 9;
	public override int Initiative => 52;
	protected override int AtlasIndex => 28;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.61930263f, 0.13961218f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					for(int i = state.ActionState.GetAbilityState<AttackAbility.State>(0).DamageDealt; i >= 0; i--)
					{
						await AbilityCmd.GenericChoice(state.Authority,
						[
							ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: _ => true,
								applyFunction: async _ =>
								{
									await MoveCharacterTokenBack(state.Performer as Character, 1);
								}, effectType: EffectType.Selectable,
								effectButtonParameters: new IconEffectButton.Parameters(CrystallizeIconPath),
								effectInfoViewParameters: new TextEffectInfoView.Parameters(
									$"Move the token on one of your {Icons.Inline(CrystallizeIconPath)} abilities backward one slot")
							),
							ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: _ => true,
								applyFunction: async _ =>
								{
									await HealAbility.Builder()
										.WithHealValue(1)
										.WithTarget(Target.Self)
										.Build()
										.Perform(state.ActionState);
								}, effectType: EffectType.Selectable,
								effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
								effectInfoViewParameters: new TextEffectInfoView.Parameters(
									$"{Icons.Inline(Icons.Heal)}1, Self")
							)
						]);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Ward);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count > 0;
				})
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.WithCustomCanApply(parameters => parameters.Figure.HasCondition(Conditions.Ward))
				.Build()),
			//Retaliate 0 ability for other things that care about retaliate abilities being performed
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(0)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.FromAttack,
						async parameters =>
						{
							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								retaliateParameters => retaliateParameters.RetaliatingFigure == state.Performer &&
								                       parameters.PotentialAbilityState == retaliateParameters.AbilityState &&
								                       RangeHelper.Distance(state.Performer.Hex, retaliateParameters.Performer.Hex) <= 1,
								async retaliateParameters =>
								{
									retaliateParameters.AdjustRetaliate(parameters.TotalShield);
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									await GDTask.CompletedTask;
								});
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithSkipConfirmation()
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 2))
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}