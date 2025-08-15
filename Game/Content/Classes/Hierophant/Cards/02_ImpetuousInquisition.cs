using System.Collections.Generic;
using Fractural.Tasks;

public class ImpetuousInquisition : HierophantCardModel<ImpetuousInquisition.CardTop, ImpetuousInquisition.CardBottom>
{
	public override string Name => "Impetuous Inquisition";
	public override int Level => 1;
	public override int Initiative => 28;
	protected override int AtlasIndex => 29 - 2;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ConditionAbility([Conditions.Disarm], range: 3, afterTargetConfirmedSubscriptions:
			[
				ScenarioEvent<ScenarioEvents.ConditionAfterTargetConfirmed.Parameters>.Subscription.New(
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
			]))
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
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
				},
				async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})),

			new AbilityCardAbility(new GrantAbility(
				figure =>
				[
					new RetaliateAbility(1)
				],
				customGetTargets: (state, list) =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(state.Performer.AlliedWith(figure))
						{
							list.Add(figure);
						}
					}
				},
				onAbilityEndedPerformed: async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				},
				conditionalAbilityCheck: state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth)))
		];

		protected override int XP => 1;
		protected override bool Round => true;
		protected override bool Loss => true;
	}
}