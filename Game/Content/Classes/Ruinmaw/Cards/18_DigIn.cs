using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DigIn : RuinmawCardModel<DigIn.CardTop, DigIn.CardBottom>
{
	public override string Name => "Dig In";
	public override int Level => 4;
	public override int Initiative => 12;
	protected override int AtlasIndex => 18;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithConditions(Ruinmaw.Empower)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.New(
						parameters => RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Any(hex => hex.HasHexObjectOfType<Obstacle>()),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(1);
							parameters.AbilityState.AbilityAddCondition(Ruinmaw.Empower);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					if(IsSated(state.Performer))
					{
						await AbilityCmd.GainXP(state.Performer, 1);
					}

					return IsSated(state.Performer);
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						state.ActionState.SetOverrideRound();

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) &&
						                      RangeHelper.Distance(state.Performer.Hex, canApplyParameters.Figure.Hex) <= 1,
						async parameters =>
						{
							if(await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible, state))
							{
								ActionState actionState = new ActionState(state.Performer,
								[
									AttackAbility.Builder().WithDamage(2).Build()
								]);
								await actionState.Perform();
							}
						}, effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Invisible)),
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove {Icons.HintText(Icons.GetCondition(Conditions.Invisible))}"));

					object subscriber = new object();
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, subscriber,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) &&
						                      RangeHelper.Distance(state.Performer.Hex, canApplyParameters.Figure.Hex) <= 1,
						async parameters =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}