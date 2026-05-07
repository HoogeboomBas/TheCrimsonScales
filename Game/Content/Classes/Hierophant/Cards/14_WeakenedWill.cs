using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WeakenedWill : HierophantLevelUpCardModel<WeakenedWill.CardTop, WeakenedWill.CardBottom>
{
	public override string Name => "Weakened Will";
	public override int Level => 2;
	public override int Initiative => 17;
	protected override int AtlasIndex => 15 - 0;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.39718983f, 0.22812192f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6233896f, 0.22812192f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.62072915f, 0.71336424f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
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
							new InfoTextExtraEffect.Parameters(textParameters => "All attacks targeting this figure this round gain disadvantage."))
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		public override bool Round => true;
	}
}