using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ContagiousMalady : BrightsparkCardModel<ContagiousMalady.CardTop, ContagiousMalady.CardBottom>
{
	public override string Name => "Contagious Malady";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 3;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.405426f, 0.22764231f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.6155555f, 0.22698413f)))
				.WithConditions(Conditions.Poison1)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithCustomGetTargets((state, targets) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					targets.AddRange(
						attackAbilityState.UniqueTargetedFigures.SelectMany(figure =>
							RangeHelper.GetFiguresInRange(figure.Hex, 1, false)));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62128145f, 0.70793647f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ConditionModel conditionGiven = null;
					List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [];
					foreach(Condition condition in state.Performer.Conditions.Where(condition => condition.ConditionModel.IsNegative))
					{
						subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								conditionGiven = condition.ConditionModel;
								await GDTask.CompletedTask;
							},
							effectType: EffectType.Selectable,
							effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(condition.ConditionModel)),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"Give {Icons.Inline(Icons.GetCondition(condition.ConditionModel))}")
						));
					}

					await AbilityCmd.GenericChoice(state.Performer, subscriptions, hintText: "Select a condition to give");
					if(conditionGiven == null)
					{
						return;
					}

					Figure figure = await AbilityCmd.SelectFigure(state,
						figures => figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							.Where(figure => figure.EnemiesWith(state.Performer))),
						hintText: () => $"Select an enemy to give {Icons.HintText(Icons.GetCondition(conditionGiven))}");
					if(figure == null)
					{
						return;
					}

					await AbilityCmd.AddCondition(state, figure, conditionGiven);
					state.SetPerformed();
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];
	}
}