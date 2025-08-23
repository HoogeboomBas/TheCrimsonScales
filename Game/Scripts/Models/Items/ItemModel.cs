using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class ItemModel : AbstractModel<ItemModel> //, IEventSubscriber
{
	private List<ItemUseSlot> _useSlots;

	public abstract string Name { get; }
	public abstract string ItemGroupId { get; }
	public abstract int ItemNumber { get; }
	public abstract int ShopCount { get; }
	public abstract int Cost { get; }
	public abstract ItemType ItemType { get; }
	public abstract ItemUseType ItemUseType { get; }

	public virtual int MinusOneCount => 0; // Amount of -1 cards this would add to the character's AMD if they do not have the ignore -1 card perk

	//public virtual List<ItemUseSlot> UseSlots { get; } = null;
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
	public int CurrentUseCountWithMaxUseCount { get; private set; } // Used for items like orbs, which can be used multiple times before being consumed without having use slots

	public bool HasUseSlots => UseSlots != null && UseSlots.Count > 0;
	public bool HasMaxUseCount => MaxUseCount > 1;

	//private bool _usable;
	private object _subscriber;
	protected ItemEffectButton.Parameters _effectButtonParameters;
	protected ItemEffectInfoView.Parameters _effectInfoViewParameters;

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

		//Subscribe();
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

	protected virtual void Subscribe()
	{
	}

	protected virtual void Unsubscribe()
	{
		ScenarioEvents.DuringAttackEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.DuringMovementEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.CardSideSelectionEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.AfterCardsPlayedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.RetaliateEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.InitiativesSortedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.LongRestCardSelectionEvent.Unsubscribe(this, _subscriber);
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

		if(fullyUsed)
		{
			switch(ItemUseType)
			{
				case ItemUseType.Spend:
					await SetItemState(ItemState.Spent);
					break;
				case ItemUseType.Consume:
					await SetItemState(ItemState.Consumed);
					break;
				case ItemUseType.Always:
					break;
				case ItemUseType.Flip:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		await apply(user);

		await ScenarioEvents.ItemUseEndedEvent.CreatePrompt(new ScenarioEvents.ItemUseEnded.Parameters(this, Owner));
	}

	protected void SubscribeDuringTurn(Func<Character, bool> canApply = null, Func<Character, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.CardSideSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.Character)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);

		ScenarioEvents.AfterCardsPlayedEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.Character)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);

		ScenarioEvents.LongRestCardSelectionEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.Character)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeDuringAttack(Func<AttackAbility.State, bool> canApply = null, Func<AttackAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.DuringAttackEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.AbilityState)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeAttackAfterTargetConfirmed(Func<AttackAbility.State, bool> canApply = null, Func<AttackAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.AbilityState)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeDuringMove(Func<MoveAbility.State, bool> canApply = null, Func<MoveAbility.State, GDTask> apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.DuringMovementEvent.Subscribe(this, _subscriber,
			canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters.AbilityState)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.AbilityState);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeSufferDamage(ScenarioEvent<ScenarioEvents.SufferDamage.Parameters>.CanApplyFunction canApply = null, ScenarioEvent<ScenarioEvents.SufferDamage.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.SufferDamageEvent.Subscribe(this, _subscriber,
			canApply: canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeRetaliate(ScenarioEvent<ScenarioEvents.Retaliate.Parameters>.CanApplyFunction canApply = null, ScenarioEvent<ScenarioEvents.Retaliate.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.RetaliateEvent.Subscribe(this, _subscriber,
			canApply: canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}

	protected void SubscribeInitiativesSorted(ScenarioEvent<ScenarioEvents.InitiativesSorted.Parameters>.CanApplyFunction canApply = null, ScenarioEvent<ScenarioEvents.InitiativesSorted.Parameters>.ApplyFunction apply = null,
		int order = 0, bool canApplyMultipleTimesDuringAbility = true)
	{
		ScenarioEvents.InitiativesSortedEvent.Subscribe(this, _subscriber,
			canApply: canApplyParameters => this.ItemState == ItemState.Available && (canApply == null || canApply(canApplyParameters)),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters);
				}
			},
			HasUseSlots ? EffectType.SelectableMandatory : EffectType.Selectable,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: _effectButtonParameters,
			effectInfoViewParameters: _effectInfoViewParameters);
	}
}