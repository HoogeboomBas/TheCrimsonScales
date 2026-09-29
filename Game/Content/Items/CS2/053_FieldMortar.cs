using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class FieldMortar : CS2Item
{
	public override string Name => "Field Mortar";
	public override int ItemNumber => 53;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 26;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		//TODO: Add some visual for which conditions are currently stored

		bool used = false;
		List<ConditionModel> conditions = [];

		ScenarioEvents.CardSideSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => CanApply(canApplyParameters.Character, used, conditions),
			async applyParameters =>
			{
				await Apply(applyParameters.Character, conditions);
				used = true;
			},
			EffectType.Selectable,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Add a condition from an adjacent figure to this item."));

		ScenarioEvents.AfterCardsPlayedEvent.Subscribe(this, _subscriber,
			canApplyParameters => CanApply(canApplyParameters.Character, used, conditions),
			async applyParameters =>
			{
				await Apply(applyParameters.Character, conditions);
				used = true;
			},
			EffectType.Selectable,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Add a condition from an adjacent figure to this item."));

		ScenarioEvents.LongRestCardSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => CanApply(canApplyParameters.Character, used, conditions),
			async applyParameters =>
			{
				await Apply(applyParameters.Character, conditions);
				used = true;
			},
			EffectType.Selectable,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Add a condition from an adjacent figure to this item."));

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			_ => used,
			async _ =>
			{
				used = false;

				await GDTask.CompletedTask;
			});

		SubscribeDuringTurn(
			canApply: character => character == Owner && conditions.Count > 0,
			apply: async character =>
			{
				await Use(async user =>
				{
					string textConditions = "";
					foreach(ConditionModel condition in conditions)
					{
						textConditions += Icons.HintText(Icons.GetCondition(condition));
					}

					Figure figure = await AbilityCmd.SelectFigure(user, figures => figures.AddRange(RangeHelper.GetFiguresInRange(user, 1, false)),
						hintText: () => "Select a figure to give " + textConditions);
					if(figure == null)
					{
						return;
					}

					await AbilityCmd.AddConditions(null, figure, conditions, user);
				});
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.CardSideSelectionEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AfterCardsPlayedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.LongRestCardSelectionEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
	}

	private bool CanApply(Character character, bool used, List<ConditionModel> conditions)
	{
		return character == Owner && !used && ItemState is ItemState.Available && RangeHelper.GetFiguresInRange(character, 1, false).Any(figure =>
			figure.Conditions.Any(condition => !conditions.Contains(condition.ConditionModel)));
	}

	private async GDTask<bool> Apply(Character user, List<ConditionModel> conditions)
	{
		Figure figure = await AbilityCmd.SelectFigure(user, figures => figures.AddRange(RangeHelper.GetFiguresInRange(user, 1, false)
				.Where(figure => figure.Conditions.Any(condition => !conditions.Contains(condition.ConditionModel)))),
			hintText: () => "Select a figure to remove a condition from");
		if(figure == null)
		{
			return false;
		}

		Condition condition = await AbilityCmd.RemoveOneCondition(null, figure, conditionModel => !conditions.Contains(conditionModel), user);
		if(condition == null)
		{
			return false;
		}

		conditions.Add(condition.ConditionModel);
		return true;
	}
}