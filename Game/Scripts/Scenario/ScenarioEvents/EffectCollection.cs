using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class EffectCollection
{
	private readonly ScenarioEvent _scenarioEvent;
	private readonly ScenarioEvent.ParametersBase _parameters;

	private bool _cleared;

	public List<Effect> Effects { get; } = new List<Effect>();
	public List<Effect> ApplicableEffects { get; } = new List<Effect>();

	public bool HasSelectableEffects => ApplicableEffects.Any(effect => effect.EffectType is EffectType.Selectable or EffectType.SelectableMandatory);

	public EffectCollection(ScenarioEvent scenarioEvent, IEnumerable<ScenarioEvent.Subscription> subscriptions, ScenarioEvent.ParametersBase parameters)
	{
		_scenarioEvent = scenarioEvent;
		_parameters = parameters;

		foreach(ScenarioEvent.Subscription subscription in subscriptions)
		{
			AddEffect(subscription);
		}

		Update();

		_scenarioEvent.SubscriptionAddedEvent += OnSubscriptionAdded;
		_scenarioEvent.SubscriptionRemovedEvent += OnSubscriptionRemoved;
	}

	public void Update()
	{
		ApplicableEffects.Clear();

		foreach(Effect effect in Effects)
		{
			effect.Update();

			if(effect.CanApply)
			{
				ApplicableEffects.Add(effect);
			}
		}
	}

	public async GDTask PerformBeforePrompt()
	{
		if(_cleared)
		{
			Log.Error("Trying to use an Effect Collection a second time. This is not allowed.");
		}

		// First, perform all Visual effects
		for(int i = 0; i < ApplicableEffects.Count; i++)
		{
			Effect effect = ApplicableEffects[i];
			if(effect.EffectType == EffectType.Visuals)
			{
				await effect.Apply();

				// Reset the current index, allowing new visual effects to be applied
				i = -1;
			}
		}

		// Then, perform all MandatoryBeforeOptionals effects
		for(int i = 0; i < ApplicableEffects.Count; i++)
		{
			Effect effect = ApplicableEffects[i];
			if(effect.EffectType == EffectType.MandatoryBeforeOptionals)
			{
				await effect.Apply();

				// Reset the current index, allowing new applicable mandatory effects to be applied
				i = -1;
			}
		}
	}

	public async GDTask PerformAfterPrompt()
	{
		// Finally, perform all mandatory after optionals effects
		for(int i = 0; i < ApplicableEffects.Count; i++)
		{
			Effect effect = ApplicableEffects[i];
			if(effect.EffectType == EffectType.MandatoryAfterOptionals)
			{
				await effect.Apply();

				// Reset the current index, allowing new applicable mandatory effects to be applied
				i = -1;
			}
		}

		// Clear any subscriptions, since this effect collection has now been used
		Clear();
	}

	private void Clear()
	{
		_cleared = true;

		_scenarioEvent.SubscriptionAddedEvent -= OnSubscriptionAdded;
		_scenarioEvent.SubscriptionRemovedEvent -= OnSubscriptionRemoved;
	}

	private void AddEffect(ScenarioEvent.Subscription subscription)
	{
		Effect effect = new Effect(subscription, _parameters, Effects.Count);
		effect.AppliedEvent += OnApplied;
		Effects.Add(effect);
	}

	private void OnApplied(Effect effect)
	{
		Update();
	}

	private void OnSubscriptionAdded(ScenarioEvent.Subscription subscription)
	{
		AddEffect(subscription);

		Update();
	}

	private void OnSubscriptionRemoved(ScenarioEvent.Subscription subscription)
	{
		for(int i = Effects.Count - 1; i >= 0; i--)
		{
			Effect effect = Effects[i];
			if(effect.Subscription == subscription)
			{
				Effects.RemoveAt(i);
			}
		}

		Update();
	}
}