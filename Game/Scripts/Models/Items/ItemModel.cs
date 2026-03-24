using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class ItemModel : AbstractModel<ItemModel>, IActionSource
{
	private readonly List<ActionState> _activeActionStates = new List<ActionState>();

	private List<ItemUseSlot> _useSlots;

	public abstract string Name { get; }
	public abstract string ItemGroupId { get; }
	public abstract int ItemNumber { get; }
	public abstract int ShopCount { get; }
	public abstract int Cost { get; }
	public abstract ItemType ItemType { get; }
	public abstract ItemUseType ItemUseType { get; }

	public virtual bool Round => false;
	public virtual bool Persistent => false;
	public virtual bool Unrecoverable => false;

	public virtual bool CanUseWhenStunned => false;

	public virtual int MinusOneCount => 0; // Amount of -1 cards this would add to the character's AMD if they do not have the ignore -1 card perk

	public virtual int SmallItemSlotCount => 0; // Amount of small item slots this would add to the character's inventory

	public virtual int MaxUseCount => 1; // Used for items like orbs, which can be used multiple times before being consumed without having use slots

	public List<ItemUseSlot> UseSlots
	{
		get
		{
			AssertMutable();

			if(_useSlots == null)
			{
				_useSlots = GetUseSlots();
			}

			return _useSlots;
		}
	}

	public Character OriginalOwner { get; private set; }
	public Character Owner { get; private set; }
	public ItemState ItemState { get; private set; }
	public int UseSlotIndex { get; private set; }

	public int CurrentUseCountWithMaxUseCount
	{
		get;
		private set;
	} // Used for items like orbs, which can be used multiple times before being consumed without having use slots

	public bool HasUseSlots => UseSlots != null && UseSlots.Count > 0;
	public bool HasMaxUseCount => MaxUseCount > 1;

	private object _subscriber;
	protected ItemEffectButton.Parameters _effectButtonParameters;
	protected ItemEffectInfoView.Parameters _effectInfoViewParameters;

	protected EffectType GetSubscriptionEffectType =>
		ItemUseType == ItemUseType.Always
			? EffectType.MandatoryBeforeOptionals
			: (HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable);

	public abstract Texture2D GetTexture();

	protected virtual List<ItemUseSlot> GetUseSlots() => [];

	public virtual void Init(Character owner)
	{
		AssertMutable();

		OriginalOwner = owner;

		_subscriber = new object();
		_effectButtonParameters = new ItemEffectButton.Parameters(this);
		_effectInfoViewParameters = new ItemEffectInfoView.Parameters(this);

		SetOwner(owner);

		ItemState = ItemState.Available;
	}

	public void SetOwner(Character character)
	{
		Unsubscribe();

		Owner = character;

		if(Owner != null)
		{
			Subscribe();
		}
	}

	public async GDTask SetItemState(ItemState state)
	{
		if(state == ItemState)
		{
			return;
		}

		ItemState oldItemState = ItemState;
		ItemState = state;

		if(oldItemState == ItemState.Available)
		{
			Unsubscribe();
		}

		if(ItemState == ItemState.Available && Owner != null)
		{
			Subscribe();
		}

		await ScenarioEvents.ItemStateChangedEvent.CreatePrompt(new ScenarioEvents.ItemStateChanged.Parameters(this));
	}

	public async GDTask Refresh()
	{
		if(ItemState == ItemState.Available)
		{
			return;
		}

		UseSlotIndex = 0;
		CurrentUseCountWithMaxUseCount = 0;

		await SetItemState(ItemState.Available);
	}

	public async GDTask RemoveFromActive()
	{
		foreach(ActionState actionState in _activeActionStates)
		{
			await actionState.RemoveFromActive();
		}

		_activeActionStates.Clear();
	}

	protected virtual void Subscribe()
	{
	}

	protected virtual void Unsubscribe()
	{
		ScenarioEvents.AbilityStartedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.DuringAttackEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.DuringMovementEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.CardSideSelectionEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AfterCardsPlayedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.RetaliateEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.InitiativesSortedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.LongRestCardSelectionEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.DuringHealEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.FigureKilledEvent.Unsubscribe(this, _subscriber);
		AbilityCmd.RemoveConditionImmunity(ScenarioEvents.GetSubscriberPair(this, _subscriber));
	}

	protected async GDTask Use(Func<Character, GDTask> apply)
	{
		await ScenarioEvents.ItemUseStartedEvent.CreatePrompt(new ScenarioEvents.ItemUseStarted.Parameters(this, Owner));

		Character user = Owner;

		bool fullyUsed = false;

		if(HasUseSlots)
		{
			ItemUseSlot oldUseSlot = UseSlots[UseSlotIndex];
			if(oldUseSlot.OnExit != null)
			{
				await oldUseSlot.OnExit(this);
			}

			UseSlotIndex++;

			if(UseSlotIndex >= UseSlots.Count)
			{
				fullyUsed = true;
			}
		}
		else
		{
			CurrentUseCountWithMaxUseCount++;

			if(CurrentUseCountWithMaxUseCount >= MaxUseCount)
			{
				fullyUsed = true;
			}
		}

		await SetItemState(ItemState.Using);

		await apply(user);

		if(fullyUsed)
		{
			if(_activeActionStates.Count > 0)
			{
				await SetItemState(ItemState.Active);
			}
			else
			{
				switch(ItemUseType)
				{
					case ItemUseType.Spend:
						await SetItemState(ItemState.Spent);
						break;
					case ItemUseType.Consume:
						await SetItemState(Unrecoverable ? ItemState.UnrecoverablyConsumed : ItemState.Consumed);
						break;
					case ItemUseType.Always:
						break;
					// case ItemUseType.Flip:
					// 	break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		await ScenarioEvents.ItemUseEndedEvent.CreatePrompt(new ScenarioEvents.ItemUseEnded.Parameters(this, Owner));
	}

	protected void SubscribeDuringTurn(Func<Character, bool> canApply = null, Func<Character, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.CardSideSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);

		ScenarioEvents.AfterCardsPlayedEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);

		ScenarioEvents.LongRestCardSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeAbilityStarted<T>(Func<T, bool> canApply = null, Func<T, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
		where T : AbilityState
	{
		ScenarioEvents.AbilityStartedEvent.Subscribe(this, _subscriber,
			parameters =>
				parameters.AbilityState is T castState &&
				(canApply == null || canApply(castState)),
			async parameters =>
			{
				if(apply != null)
				{
					await apply((T)parameters.AbilityState);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeTurnEnded(Func<Figure, bool> canApply = null, Func<Figure, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Figure),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Figure);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeDuringHeal(Func<HealAbility.State, bool> canApply = null, Func<HealAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.DuringHealEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.AbilityState),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeDuringAttack(Func<AttackAbility.State, bool> canApply = null, Func<AttackAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.AbilityState),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeAttackAfterTargetConfirmed(Func<AttackAbility.State, bool> canApply = null,
		Func<AttackAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.AbilityState),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeAMDCardDrawn(Func<ScenarioEvents.AMDCardDrawn.Parameters, bool> canApply = null,
		Func<ScenarioEvents.AMDCardDrawn.Parameters, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.AMDCardDrawnEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeDuringMove(Func<MoveAbility.State, bool> canApply = null, Func<MoveAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.DuringMovementEvent.Subscribe(this, _subscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.AbilityState),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeSufferDamage(ScenarioEvent<ScenarioEvents.SufferDamage.Parameters>.CanApplyFunction canApply = null,
		ScenarioEvent<ScenarioEvents.SufferDamage.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.SufferDamageEvent.Subscribe(this, _subscriber,
			canApply,
			apply,
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeRetaliate(ScenarioEvent<ScenarioEvents.Retaliate.Parameters>.CanApplyFunction canApply = null,
		ScenarioEvent<ScenarioEvents.Retaliate.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.RetaliateEvent.Subscribe(this, _subscriber,
			canApply,
			apply,
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeInitiativesSorted(ScenarioEvent<ScenarioEvents.InitiativesSorted.Parameters>.CanApplyFunction canApply = null,
		ScenarioEvent<ScenarioEvents.InitiativesSorted.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.InitiativesSortedEvent.Subscribe(this, _subscriber,
			canApply,
			apply,
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeFigureKilled(ScenarioEvent<ScenarioEvents.FigureKilled.Parameters>.CanApplyFunction canApply = null,
		ScenarioEvent<ScenarioEvents.FigureKilled.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.FigureKilledEvent.Subscribe(this, _subscriber,
			canApply,
			apply,
			GetSubscriptionEffectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeConditionImmunity(ConditionModel conditionModel,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		AbilityCmd.AddConditionImmunity(ScenarioEvents.GetSubscriberPair(this, _subscriber), conditionModel, Owner);
	}

	protected void SubscribeConditionsImmunity(List<ConditionModel> conditionModels,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		AbilityCmd.AddConditionsImmunity(ScenarioEvents.GetSubscriberPair(this, _subscriber), conditionModels, Owner);
	}

	protected ActionState GetActionState(Figure performer, Ability[] abilities)
	{
		ActionState actionState = new ActionState(this, performer, abilities, //null, 
			onFirstActivateAbilityActivated: OnFirstActivateAbilityActivated, onDiscardOrLoseRequested: OnDiscardOrLoseRequested);

		return actionState;
	}

	private async GDTask OnFirstActivateAbilityActivated(ActionState actionState)
	{
		_activeActionStates.Add(actionState);

		await GDTask.CompletedTask;
	}

	private async GDTask OnDiscardOrLoseRequested(ActionState actionState)
	{
		await AbilityCmd.SpendOrConsume(this);
	}
}