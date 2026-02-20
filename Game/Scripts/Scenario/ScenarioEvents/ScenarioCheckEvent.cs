using System;
using System.Collections.Generic;
using Godot;

public class ScenarioCheckEvent<T> : ScenarioCheckEvent
	where T : ScenarioCheckEvent.ParametersBase
{
	private new class Subscription : ScenarioCheckEvent.Subscription
	{
		public CanApplyFunction CanApplyFunction { get; set; }

		public ApplyFunction ApplyFunction { get; init; }

		public override bool CanApply(ParametersBase parameters)
		{
			return CanApplyFunction == null || CanApplyFunction.Invoke((T)parameters);
		}

		public override void Apply(ParametersBase parameters)
		{
			if(ApplyFunction != null)
			{
				ApplyFunction.Invoke((T)parameters);
			}
		}
	}

	private readonly List<Subscription> _subscriptions = new List<Subscription>();

	public delegate bool CanApplyFunction(T parameters);

	public delegate void ApplyFunction(T parameters);

	public event Action SubscribersChangedEvent;

	public void FireChangedEvent()
	{
		SubscribersChangedEvent?.Invoke();
	}

	public T Fire(T parameters)
	{
		foreach(Subscription subscription in _subscriptions)
		{
			if(subscription.CanApply(parameters))
			{
				subscription.Apply(parameters);
			}
		}

		return parameters;
	}

	public void Subscribe(Figure subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		int order = 0)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, order);
	}

	public void Subscribe(AbilityState subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		int order = 0)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, order);
	}

	public void Subscribe(ItemModel subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		int order = 0)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, order);
	}

	public void Subscribe(ScenarioModel subscriberA, object subscriberB,
		CanApplyFunction canApply, ApplyFunction apply,
		int order = 0)
	{
		Subscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB), canApply, apply, order);
	}

	public void Subscribe(IEventSubscriber subscriber,
		CanApplyFunction canApply, ApplyFunction apply,
		int order = 0)
	{
		Subscription newSubscription = new Subscription()
		{
			CanApplyFunction = canApply,
			ApplyFunction = apply,
			//EffectType = effectType,
			Order = order,
		};

		newSubscription.SetSubscriber(subscriber);

		foreach(Subscription subscription in _subscriptions)
		{
			if(subscription.Subscriber == subscriber)
			{
				Log.Error("Trying to subscribe to an event already subscribed to by this subscriber. This is probably wrong!");
				return;
			}
		}

		bool inserted = false;
		for(int i = 0; i < _subscriptions.Count; i++)
		{
			if(order < _subscriptions[i].Order)
			{
				_subscriptions.Insert(i, newSubscription);
				inserted = true;
				break;
			}
		}

		if(!inserted)
		{
			_subscriptions.Add(newSubscription);
		}

		FireChangedEvent();
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

	public void Unsubscribe(ItemModel subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(ScenarioModel subscriberA, object subscriberB)
	{
		Unsubscribe(ScenarioEvents.GetSubscriberPair(subscriberA, subscriberB));
	}

	public void Unsubscribe(IEventSubscriber subscriber)
	{
		for(int i = _subscriptions.Count - 1; i >= 0; i--)
		{
			if(_subscriptions[i].Subscriber == subscriber)
			{
				_subscriptions.RemoveAt(i);
			}
		}

		FireChangedEvent();
	}
}

public abstract class ScenarioCheckEvent
{
	public abstract class Subscription
	{
		public IEventSubscriber Subscriber { get; private set; }

		public int Order { get; set; }

		//public EffectType EffectType { get; set; }

		public abstract bool CanApply(ParametersBase parameters);

		public abstract void Apply(ParametersBase parameters);

		public void SetSubscriber(IEventSubscriber subscriber)
		{
			Subscriber = subscriber;
		}
	}

	public abstract class ParametersBase
	{
	}
}