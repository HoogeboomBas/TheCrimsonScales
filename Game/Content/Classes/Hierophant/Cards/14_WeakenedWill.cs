using System.Collections.Generic;
using Fractural.Tasks;

public class WeakenedWill : HierophantLevelUpCardModel<WeakenedWill.CardTop, WeakenedWill.CardBottom>
{
	public override string Name => "Weakened Will";
	public override int Level => 2;
	public override int Initiative => 17;
	protected override int AtlasIndex => 15 - 0;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(3)
				.WithConditions(Conditions.Muddle)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithCustomGetTargets((state, list) =>
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
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(1).Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(state =>
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
						parameters => parameters.SetDisadvantage(true)
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure),
						parameters => parameters.Add(
							new FigureInfoTextExtraEffect.Parameters("All attacks targeting this figure this round gain disadvantage."))
					);

					return GDTask.CompletedTask;
				})
				.WithOnDeactivate(state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

						return GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override bool Round => true;
	}
}