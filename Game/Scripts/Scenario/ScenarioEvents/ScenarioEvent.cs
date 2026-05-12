using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class ScenarioEvent<T> : ScenarioEvent
	where T : ScenarioEvent.ParametersBase
{
	public new class Subscription : ScenarioEvent.Subscription
	{
		private CanApplyFunction _extraCanApplyFunction = null;
		private bool _hasBeenAppliedDuringSubscription;

		public CanApplyFunction CanApplyFunction { get; }

		public ApplyFunction ApplyFunction { get; }

		private Subscription(CanApplyFunction canApplyFunction, ApplyFunction applyFunction, EffectType effectType,
			int order, bool canApplyMultipleTimesDuringSubscription, bool canApplyMultipleTimesInEffectCollection,
			EffectButtonParameters effectButtonParameters, EffectInfoViewParameters effectInfoViewParameters)
		{
			CanApplyFunction = canApplyFunction;
			ApplyFunction = applyFunction;
			EffectType = effectType;
			Order = order;
			CanApplyMultipleTimesDuringSubscription = canApplyMultipleTimesDuringSubscription;
			CanApplyMultipleTimesInEffectCollection = canApplyMultipleTimesInEffectCollection;
			EffectButtonParameters = effectButtonParameters;
			EffectInfoViewParameters = effectInfoViewParameters;
		}

		public static Subscription New(CanApplyFunction canApplyFunction = null, ApplyFunction applyFunction = null,
			EffectType effectType = EffectType.MandatoryBeforeOptionals,
			int order = 0, bool canApplyMultipleTimesDuringSubscription = true, bool canApplyMultipleTimesInEffectCollection = false,
			EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null)
		{
			return new Subscription(canApplyFunction, applyFunction, effectType, order, canApplyMultipleTimesDuringSubscription,
				canApplyMultipleTimesInEffectCollection,
				effectButtonParameters ?? new IconEffectButton.Parameters(Icons.GetItem(ItemType.Head)),
				effectInfoViewParameters ?? new TextEffectInfoView.Parameters("TODO"));
		}

		public static Subscription ConsumeElement(List<CardElementConsumption> elements, CanApplyFunction canApplyFunction = null,
			ApplyFunction applyFunction = null,
			EffectType effectType = EffectType.Selectable,
			int order = 0, bool canApplyMultipleTimesDuringSubscription = false, bool canApplyMultipleTimesInEffectCollection = false,
			EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
			Figure potentialConsumer = null)
		{
			//TODO: Make sure this works for items that make you skip an element consumption (perhaps after clicking, a new prompt opens up to select what to use)
			return new Subscription(parameters =>
				{
					if(parameters is ParametersBaseWithAbilityState parametersBase)
					{
						potentialConsumer ??= parametersBase.BaseAbilityState.Performer;
					}

					return AbilityCmd.CanConsumeElements(elements, potentialConsumer) &&
					       (canApplyFunction == null || canApplyFunction.Invoke(parameters));
				},
				async parameters =>
				{
					Figure potentialConsumer = null;
					if(parameters is ParametersBaseWithAbilityState parametersBase)
					{
						potentialConsumer = parametersBase.BaseAbilityState.Performer;
					}

					await AbilityCmd.ConsumeElements(potentialConsumer, elements);
					if(applyFunction != null)
					{
						await applyFunction.Invoke(parameters);
					}
				}, effectType, order, canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection,
				effectButtonParameters ?? new ConsumeElementEffectButton.Parameters(elements),
				effectInfoViewParameters ?? new TextEffectInfoView.Parameters("TODO"));
		}

		public static Subscription ConsumeElement(Element element, CanApplyFunction canApplyFunction = null,
			ApplyFunction applyFunction = null,
			EffectType effectType = EffectType.Selectable,
			int order = 0, bool canApplyMultipleTimesDuringSubscription = false, bool canApplyMultipleTimesInEffectCollection = false,
			EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null)
		{
			return ConsumeElement([CardElementConsumption.Consume(element)], canApplyFunction, applyFunction, effectType, order,
				canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters);
		}

		public override bool CanApply(ParametersBase parameters)
		{
			if(!Subscribed)
			{
				return false;
			}

			if(_hasBeenAppliedDuringSubscription && !CanApplyMultipleTimesDuringSubscription)
			{
				return false;
			}

			T castParameters = (T)parameters;

			if(_extraCanApplyFunction != null && !_extraCanApplyFunction(castParameters))
			{
				return false;
			}

			return CanApplyFunction == null || CanApplyFunction.Invoke(castParameters);
		}

		public override async GDTask Apply(ParametersBase parameters)
		{
			_hasBeenAppliedDuringSubscription = true;

			if(ApplyFunction != null)
			{
				await ApplyFunction.Invoke((T)parameters);
			}
		}

		public void SetExtraCanApplyFunction(CanApplyFunction canApplyFunction)
		{
			_extraCanApplyFunction = canApplyFunction;
		}

		public void ClearSubscriptionAppliedAndExtraCanApplyFunctions()
		{
			_hasBeenAppliedDuringSubscription = false;
			_extraCanApplyFunction = null;
		}
	}

	private readonly List<Subscription> _subscriptions = new List<Subscription>();

	public delegate bool CanApplyFunction(T parameters);

	public delegate GDTask ApplyFunction(T parameters);

	public EffectCollection CreateEffectCollection(T parameters)
	{
		EffectCollection collection = new EffectCollection(this, _subscriptions, parameters);
		return collection;
	}

	public GDTask<T> CreatePrompt(T parameters)
	{
		if(parameters is ParametersBaseWithAbilityState parametersBaseWithAbilityState && parametersBaseWithAbilityState.BaseAbilityState != null)
		{
			return CreatePrompt(parameters, parametersBaseWithAbilityState.BaseAbilityState);
		}

		return CreatePrompt(parameters, GameController.Instance.CharacterManager.GetCharacter(0));
	}

	public GDTask<T> CreatePrompt(T parameters, AbilityState state)
	{
		return CreatePrompt(parameters, state.Authority);
	}

	public async GDTask<T> CreatePrompt(T parameters, Figure authority, string hintText = "Select effects to use")
	{
		EffectCollection collection = CreateEffectCollection(parameters);

		await CreatePrompt(collection, authority, hintText);

		return parameters;
	}

	public async GDTask CreatePrompt(EffectCollection collection, Figure authority, string hintText = "Select effects to use")
	{
		await PromptManager.Prompt(new ScenarioEventPrompt(collection, () => hintText), authority);
	}

	public void Subscribe(Figure subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimesDuringSubscription = true,
		bool canApplyMultipleTimesInEffectCollection = false,
		EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
		bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, effectType, order,
			canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters,
			checkDuplicates);
	}

	public void Subscribe(Figure subscriberA, object subscriberB, Subscription subscription, bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), subscription, checkDuplicates);
	}

	public void Subscribe(AbilityState subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimesDuringSubscription = true,
		bool canApplyMultipleTimesInEffectCollection = false,
		EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
		bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, effectType, order,
			canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters,
			checkDuplicates);
	}

	public void Subscribe(AbilityState subscriberA, object subscriberB, Subscription subscription, bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), subscription, checkDuplicates);
	}

	public void Subscribe(ScenarioModel subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimesDuringSubscription = true,
		bool canApplyMultipleTimesInEffectCollection = false,
		EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
		bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, effectType, order,
			canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters,
			checkDuplicates);
	}

	public void Subscribe(ScenarioModel subscriberA, object subscriberB, Subscription subscription, bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), subscription, checkDuplicates);
	}

	public void Subscribe(ItemModel subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimesDuringSubscription = true,
		bool canApplyMultipleTimesInEffectCollection = false,
		EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
		bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, effectType, order,
			canApplyMultipleTimesDuringSubscription, canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters,
			checkDuplicates);
	}

	public void Subscribe(ItemModel subscriberA, object subscriberB, Subscription subscription, bool checkDuplicates = true)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), subscription, checkDuplicates);
	}

	public void Subscribe(IEventSubscriber subscriber,
		CanApplyFunction canApply, ApplyFunction apply,
		EffectType effectType = EffectType.MandatoryBeforeOptionals, int order = 0, bool canApplyMultipleTimesDuringSubscription = true,
		bool canApplyMultipleTimesInEffectCollection = false,
		EffectButtonParameters effectButtonParameters = null, EffectInfoViewParameters effectInfoViewParameters = null,
		bool checkDuplicates = true)
	{
		Subscription subscription = Subscription.New(canApply, apply, effectType, order, canApplyMultipleTimesDuringSubscription,
			canApplyMultipleTimesInEffectCollection, effectButtonParameters, effectInfoViewParameters);

		Subscribe(subscriber, subscription, checkDuplicates);
	}

	public void Subscribe(AbilityState abilityState, object subscriberB, List<Subscription> subscriptions)
	{
		if(subscriptions != null)
		{
			foreach(Subscription subscription in subscriptions)
			{
				subscription.ClearSubscriptionAppliedAndExtraCanApplyFunctions();

				subscription.SetExtraCanApplyFunction(parameters =>
				{
					if(parameters is not ParametersBaseWithAbilityState parametersBaseWithAbilityState)
					{
						Log.Error("Trying to subscribe a list for a specific ability state, but the event does not support an ability state");
						return false;
					}

					if(parametersBaseWithAbilityState.BaseAbilityState != abilityState)
					{
						return false;
					}

					return true;
				});

				Subscribe(ScenarioEvents.GetSubscriberPair(abilityState, subscriberB), subscription, false);
			}
		}
	}

	public void Subscribe(IEventSubscriber subscriber, Subscription subscription, bool checkDuplicates = true)
	{
		if(checkDuplicates && subscription.Subscribed)
		{
			Log.Error("Trying to subscribe to an event already subscribed to by this subscriber. This is probably wrong!");
			return;
		}

		subscription.SetSubscriber(subscriber);
		subscription.SetSubscribed(true);

		if(checkDuplicates)
		{
			foreach(Subscription otherSubscription in _subscriptions)
			{
				if(otherSubscription.Subscriber == subscriber)
				{
					Log.Error("Trying to subscribe to an event already subscribed to by this subscriber. This is probably wrong!");
					return;
				}
			}
		}

		bool inserted = false;
		for(int i = 0; i < _subscriptions.Count; i++)
		{
			if(subscription.Order < _subscriptions[i].Order)
			{
				_subscriptions.Insert(i, subscription);
				inserted = true;
				break;
			}
		}

		if(!inserted)
		{
			_subscriptions.Add(subscription);
		}

		FireSubscriptionAddedEvent(subscription);
	}

	private void Unsubscribe(Subscription subscription)
	{
		Unsubscribe(subscription.Subscriber);
	}

	public void Unsubscribe(Figure subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(AbilityState subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(ScenarioModel subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(ItemModel subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(IEventSubscriber subscriber)
	{
		for(int i = _subscriptions.Count - 1; i >= 0; i--)
		{
			Subscription subscription = _subscriptions[i];
			if(subscription.Subscriber == subscriber)
			{
				subscription.SetSubscribed(false);
				subscription.ClearSubscriptionAppliedAndExtraCanApplyFunctions();
				_subscriptions.RemoveAt(i);

				FireSubscriptionRemovedEvent(subscription);
			}
		}
	}

	public void Unsubscribe(List<Subscription> subscriptions)
	{
		if(subscriptions != null)
		{
			foreach(Subscription subscription in subscriptions)
			{
				Unsubscribe(subscription);
			}
		}
	}

	public void ClearAllSubscriptions()
	{
		_subscriptions.Clear();
	}
}

public abstract class ScenarioEvent
{
	public abstract class Subscription
	{
		public IEventSubscriber Subscriber { get; private set; }
		public bool Subscribed { get; private set; }

		public EffectType EffectType { get; set; }
		public int Order { get; init; }
		public bool CanApplyMultipleTimesDuringSubscription { get; init; }
		public bool CanApplyMultipleTimesInEffectCollection { get; init; }

		public EffectButtonParameters EffectButtonParameters { get; init; }
		public EffectInfoViewParameters EffectInfoViewParameters { get; init; }

		public abstract bool CanApply(ParametersBase parameters);

		public abstract GDTask Apply(ParametersBase parameters);

		public void SetSubscriber(IEventSubscriber subscriber)
		{
			Subscriber = subscriber;
		}

		public void SetSubscribed(bool subscribed)
		{
			Subscribed = subscribed;
		}
	}

	public abstract class ParametersBase<T> : ParametersBaseWithAbilityState
		where T : AbilityState
	{
		public T AbilityState { get; }

		public override AbilityState BaseAbilityState => AbilityState;
		public Figure Authority { get; private set; }
		public Figure Performer => AbilityState.Performer;

		public ParametersBase(T abilityState)
		{
			if(abilityState == null)
			{
				Log.Error($"Ability State cannot be null! {this.GetType()}");
				return;
			}

			AbilityState = abilityState;
			Authority = AbilityState.Authority;
		}

		public void SetAuthority(Figure figure)
		{
			Authority = figure;
		}
	}

	public abstract class ParametersBaseWithAbilityState : ParametersBase
	{
		public abstract AbilityState BaseAbilityState { get; }
	}

	// public abstract class AbilityStateParametersBase(AbilityState abilityState) : ParametersBase
	// {
	// 	public AbilityState AbilityState { get; } = abilityState;
	//
	// 	//public Figure Authority => AbilityState?.Performer;
	// 	//public Figure Performer => AbilityState?.Performer;
	// }

	public abstract class ParametersBase
	{
	}

	public event Action<Subscription> SubscriptionAddedEvent;

	protected void FireSubscriptionAddedEvent(Subscription subscription)
	{
		SubscriptionAddedEvent?.Invoke(subscription);
	}

	public event Action<Subscription> SubscriptionRemovedEvent;

	protected void FireSubscriptionRemovedEvent(Subscription subscription)
	{
		SubscriptionRemovedEvent?.Invoke(subscription);
	}
}