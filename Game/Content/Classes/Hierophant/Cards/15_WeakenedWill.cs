using System.Collections.Generic;
using Fractural.Tasks;

public class WeakenedWill : HierophantCardModel<WeakenedWill.CardTop, WeakenedWill.CardBottom>
{
	public override string Name => "Weakened Will";
	public override int Level => 2;
	public override int Initiative => 17;
	protected override int AtlasIndex => 29 - 15;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(2, range: 3, conditions: [Conditions.Muddle])),

			new AbilityCardAbility(new ConditionAbility([Conditions.Strengthen],
				customGetTargets: (state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					foreach(Figure targetedFigure in attackAbilityState.UniqueTargetedFigures)
					{
						if(!targetedFigure.IsDead)
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(targetedFigure.Hex, 1))
							{
								if(state.Performer.AlliedWith(figure))
								{
									list.AddIfNew(figure);
								}
							}
						}
					}
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(1).Build()),

			new AbilityCardAbility(new OtherActiveAbility(
				state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.AbilityState.Target),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasDisadvantage();

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Target),
						parameters => parameters.SetDisadvantage()
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure),
						parameters => parameters.Add(new FigureInfoTextExtraEffect.Parameters("All attacks targeting this figure this round gain disadvantage."))
					);

					return GDTask.CompletedTask;
				},
				state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

					return GDTask.CompletedTask;
				}
			))
		];

		protected override bool Round => true;
	}
}