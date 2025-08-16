using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// Ability State contains all the results and status of performing an Ability.
/// </summary>
public abstract class AbilityState
{
	private bool _blocked;
	private Dictionary<object, Dictionary<string, object>> _customValues = new Dictionary<object, Dictionary<string, object>>();

	//TODO: Change this into a weak reference to make sure GC works correctly because of cyclic referencing?
	public ActionState ActionState { get; init; }

	public bool Performed { get; private set; }

	public Figure Performer => ActionState.Performer;
	public Figure Authority => ActionState.Authority;

	public bool PerformWhileStunned { get; set; } = false;
	public bool Blocked => _blocked || Performer.IsDead;

	public void SetPerformed()
	{
		Performed = true;
	}

	public void SetBlocked()
	{
		_blocked = true;
	}

	public virtual GDTask RemoveFromActive()
	{
		return GDTask.CompletedTask;
	}

	public void SetCustomValue(object source, string key, object value)
	{
		if(!_customValues.TryGetValue(source, out Dictionary<string, object> sourceValues))
		{
			sourceValues = new Dictionary<string, object>();
			_customValues.Add(source, sourceValues);
		}

		sourceValues[key] = value;
	}

	public T GetCustomValue<T>(object source, string key)
	{
		if(!_customValues.TryGetValue(source, out Dictionary<string, object> sourceValues) || !sourceValues.TryGetValue(key, out object value))
		{
			//Log.Error($"Could not find custom value for: {source} with key: {key}");
			return default;
		}

		if(value is not T castValue)
		{
			Log.Error($"Could not cast custom value for: {source} with key: {key}");
			return default;
		}

		return castValue;
	}

	public bool TryGetCustomValue<T>(object source, string key, out T value)
	{
		if(!_customValues.TryGetValue(source, out Dictionary<string, object> sourceValues) || !sourceValues.TryGetValue(key, out object retrievedValue))
		{
			//Log.Error($"Could not find custom value for: {source} with key: {key}");
			value = default;
			return false;
		}

		if(retrievedValue is not T castValue)
		{
			Log.Error($"Could not cast custom value for: {source} with key: {key}");
			value = default;
			return false;
		}

		value = castValue;
		return true;
	}
}