using System;
using Fractural.Tasks;
using Godot;

public class Effect
{
	private ScenarioEvent.ParametersBase _parameters;

	public ScenarioEvent.Subscription Subscription { get; private set; }
	public int Index { get; private set; }

	public bool CanApply { get; private set; }
	public bool Applied { get; private set; }

	public EffectType EffectType => Subscription.EffectType;

	public event Action<Effect> AppliedEvent;

	public Effect(ScenarioEvent.Subscription subscription, ScenarioEvent.ParametersBase parameters, int index)
	{
		Subscription = subscription;
		_parameters = parameters;
		Index = index;
	}

	public void Update()
	{
		CanApply = Subscription.CanApply(_parameters) && (Subscription.CanApplyMultipleTimesInEffectCollection || !Applied);
	}

	public void UpdateIndex(int index)
	{
		Index = index;
	}

	public async GDTask Apply()
	{
		if(!CanApply)
		{
			Log.Error("Trying to apply an effect that cannot be applied.");
			return;
		}

		await Subscription.Apply(_parameters);

		Applied = true;
		AppliedEvent?.Invoke(this);
	}
}