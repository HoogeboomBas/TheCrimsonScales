using System.Collections.Generic;
using Fractural.Tasks;

public class ImpetuousInquisition : HierophantCardModel<ImpetuousInquisition.CardTop, ImpetuousInquisition.CardBottom>
{
	public override string Name => "Impetuous Inquisition";
	public override int Level => 1;
	public override int Initiative => 28;
	protected override int AtlasIndex => 13 - 2;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithRange(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.ConditionAfterTargetConfirmed.Subscription.New(
						canApplyFunction: canApplyParameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(canApplyParameters.AbilityState.Target.Hex, 1))
							{
								if(canApplyParameters.AbilityState.Performer.AlliedWith(figure))
								{
									return true;
								}
							}

							return false;
						},
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Curse);
							await AbilityCmd.GainXP(parameters.Performer, 1);

							await GDTask.CompletedTask;
						})
				)
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						canApply: canApplyParameters =>
						{
							return
								canApplyParameters.SufferDamageParameters.FromAttack &&
								state.Performer.EnemiesWith(canApplyParameters.SufferDamageParameters.PotentialAttackAbilityState.Performer) &&
								state.Performer.AlliedWith(canApplyParameters.SufferDamageParameters.Figure) &&
								canApplyParameters.Damage >= 3;
						},
						apply: async applyParameters =>
						{
							await AbilityCmd.SufferDamage(null, applyParameters.PotentialAttackAbilityState.Performer, 2);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
					[
						RetaliateAbility.Builder().WithRetaliateValue(1).Build()
					]
				)
				.WithCustomGetTargets((state, list) =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(state.Performer.AlliedWith(figure))
						{
							list.Add(figure);
						}
					}
				})
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
				.Build())
		];

		protected override int XP => 1;
		protected override bool Round => true;
		protected override bool Loss => true;
	}
}