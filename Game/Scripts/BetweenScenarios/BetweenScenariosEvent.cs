using System.Collections.Generic;
using System.Linq;
using Godot;

public class BetweenScenariosEvent<T> : BetweenScenariosEvent
	where T : BetweenScenariosEvent.ParametersBase
{
	private new class Subscription : BetweenScenariosEvent.Subscription
	{
		public ApplyFunction ApplyFunction { get; init; }

		public override void Apply(ParametersBase parameters)
		{
			if(ApplyFunction != null)
			{
				ApplyFunction.Invoke((T)parameters);
			}
		}
	}

	private readonly List<Subscription> _subscriptions = new List<Subscription>();

	public delegate void ApplyFunction(T parameters);

	public T Fire(T parameters)
	{
		List<Subscription> reversedSubscriptions = _subscriptions.ToList();
		reversedSubscriptions.Reverse();
		for(int i = reversedSubscriptions.Count - 1; i >= 0; i--)
		{
			Subscription subscription = reversedSubscriptions[i];
			subscription.Apply(parameters);
		}

		return parameters;
	}

	public void Subscribe(object subscriber, ApplyFunction apply, int order = 0)
	{
		Subscription newSubscription = new Subscription()
		{
			Subscriber = subscriber,
			ApplyFunction = apply,
			Order = order,
		};

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
	}

	private void Unsubscribe(Subscription subscription)
	{
		Unsubscribe(subscription.Subscriber);
	}

	public void Unsubscribe(object subscriber)
	{
		for(int i = _subscriptions.Count - 1; i >= 0; i--)
		{
			if(_subscriptions[i].Subscriber == subscriber)
			{
				_subscriptions.RemoveAt(i);
			}
		}
	}
}

public abstract class BetweenScenariosEvent
{
	protected abstract class Subscription
	{
		public object Subscriber { get; init; }

		public int Order { get; init; }

		public abstract void Apply(ParametersBase parameters);
	}

	public abstract class ParametersBase
	{
	}
}