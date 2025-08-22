using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// A <see cref="TargetedAbility{T, TSingleTargetState}"/> that gives an item to another character.
/// </summary>
public class GiveItemAbility : TargetedAbility<GiveItemAbility.State, SingleTargetState>
{
	public class State : TargetedAbilityState<SingleTargetState>
	{
	}

	protected Action<AbilityState, List<ItemModel>> _getItems { get; set; }
	protected Func<AbilityState, ItemModel, GDTask> _onItemGiven { get; set; }
	protected Func<ItemModel, GDTask> _onItemConsumed { get; set; }

	// TODO varadski 21.08.2025: also exists on GiveItemAbility; move to TargetAbility?
	protected bool _selectAutomatically { get; set; }

	/// <summary>
	/// A builder extending <see cref="TargetedAbility{T, TSingleTargetState}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in GiveItemAbility. Enables inheritors of GiveItemAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending GiveItemAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : TargetedAbility<State, SingleTargetState>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGetItemsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : GiveItemAbility, new()
	{
		public interface IGetItemsStep
		{
			TBuilder WithGetItems(Action<AbilityState, List<ItemModel>> getItems);
		}

		public TBuilder WithGetItems(Action<AbilityState, List<ItemModel>> getItems)
		{
			Obj._getItems = getItems;
			return (TBuilder)this;
		}

		public TBuilder WithOnItemGiven(Func<AbilityState, ItemModel, GDTask> onItemGiven)
		{
			Obj._onItemGiven = onItemGiven;
			return (TBuilder)this;
		}

		public TBuilder WithOnItemConsumed(Func<ItemModel, GDTask> onItemConsumed)
		{
			Obj._onItemConsumed = onItemConsumed;
			return (TBuilder)this;
		}

		public TBuilder WithSelectAutomatically(bool selectAutomatically)
		{
			Obj._selectAutomatically = selectAutomatically;
			return (TBuilder)this;
		}

		/// <summary>
		/// Overriding so we can set default values.
		/// </summary>
		public override TAbility Build()
		{
			Obj.Target = _target ?? Target.Allies | Target.MustTargetCharacters;
			return base.Build();
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class GiveItemBuilder : AbstractBuilder<GiveItemBuilder, GiveItemAbility>
	{
		internal GiveItemBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of GiveItemBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<GiveItemBuilder, GiveItemAbility>.IGetItemsStep Builder()
	{
		return new GiveItemBuilder();
	}

	public GiveItemAbility() { }

	public GiveItemAbility(Action<AbilityState, List<ItemModel>> getItems,
		Func<AbilityState, ItemModel, GDTask> onItemGiven = null,
		Func<ItemModel, GDTask> onItemConsumed = null,
		bool selectAutomatically = false,
		int targets = 1, int? range = null, RangeType? rangeType = null,
		Target target = Target.Allies | Target.MustTargetCharacters,
		bool requiresLineOfSight = true, bool mandatory = false,
		Hex targetHex = null,
		AOEPattern aoePattern = null, int push = 0, int pull = 0, ConditionModel[] conditions = null,
		Action<State, List<Figure>> customGetTargets = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getTargetingHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(targets, range, rangeType, target,
			requiresLineOfSight, mandatory, targetHex, aoePattern, push, pull, conditions,
			customGetTargets, onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed,
			conditionalAbilityCheck, getTargetingHintText, abilityStartedSubscriptions, abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		_getItems = getItems;
		_onItemGiven = onItemGiven;
		_onItemConsumed = onItemConsumed;

		_selectAutomatically = selectAutomatically;
	}

	protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
	{
		await base.AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

		if(target is not Character character)
		{
			Log.Error("Trying to give an item to a figure that isn't a character.");
			return;
		}

		await GiveItem(abilityState, character, _getItems, _onItemGiven, _onItemConsumed, _selectAutomatically);
	}

	public static async GDTask GiveItem(AbilityState abilityState, Character target, Action<AbilityState, List<ItemModel>> getItems,
		Func<AbilityState, ItemModel, GDTask> onItemGiven, Func<ItemModel, GDTask> onItemConsumed, bool selectAutomatically = false)
	{
		ItemModel item;
		List<ItemModel> items = new List<ItemModel>();
		getItems(abilityState, items);

		if(selectAutomatically)
		{
			item = items.Count == 0 ? null : items[0];
		}
		else
		{
			item = await AbilityCmd.SelectItem(abilityState.Authority, items, "Select an item to give");
		}

		if(item != null && target is Character character)
		{
			if(onItemGiven != null)
			{
				await onItemGiven(abilityState, item);
			}

			character.AddItem(item);

			abilityState.SetPerformed();

			object subscriber = new object();
			ScenarioEvents.ItemStateChangedEvent.Subscribe(abilityState, subscriber,
				parameters => parameters.Item == item,
				async parameters =>
				{
					if(item.ItemState == ItemState.Consumed)
					{
						if(onItemConsumed != null)
						{
							ScenarioEvents.ItemStateChangedEvent.Unsubscribe(abilityState, subscriber);

							await onItemConsumed(item);
						}
					}
				}
			);
		}
	}
}