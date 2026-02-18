using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Condition : IEventSubscriber
{
	private readonly EventSubscriberPair _subscriberPair;
	private readonly Dictionary<string, object> _customValues = new Dictionary<string, object>();

	private bool _appliedDuringThisTurn;

	public ConditionModel ConditionModel { get; private set; }
	public ConditionHexObjectEffectView EffectView { get; private set; }
	public Figure Owner { get; private set; }
	public Figure PotentialGiver { get; private set; }

	public int StackCount { get; private set; }

	public Condition(ConditionModel conditionModel, Figure owner, Figure potentialGiver)
	{
		ConditionModel = conditionModel;
		Owner = owner;
		PotentialGiver = potentialGiver;
		StackCount = 1;

		if(conditionModel.RequiresGiver && potentialGiver == null)
		{
			Log.Error($"Trying to add {conditionModel.Name} to {owner.DisplayName}, but {nameof(potentialGiver)} is null.");
		}

		// Create a custom subscriber pair, so it can't interfere with subscriptions added by Condition Models
		_subscriberPair = ScenarioEvents.GetSubscriberPair(this, new object());
	}

	public async GDTask OnAdded()
	{
		if(ConditionModel.ShouldShowOnFigure)
		{
			EffectView = Owner.AddEffectView<ConditionHexObjectEffectView>(new ConditionHexObjectEffectView.Parameters(ConditionModel));
			EffectView.SetStackCount(ConditionModel.UpgradableLevel);
		}

		if(Owner.TakingTurn)
		{
			_appliedDuringThisTurn = true;
		}

		ScenarioEvents.InflictConditionDuplicatesCheckEvent.Subscribe(_subscriberPair,
			parameters =>
			{
				bool sameBaseCondition = parameters.ConditionModel.BaseLevelCondition.Equals(ConditionModel.BaseLevelCondition);

				if(parameters.Prevented || parameters.Target != Owner)
				{
					return false;
				}

				if(!sameBaseCondition)
				{
					return false;
				}

				// Block either the exact same condition, or the same condition of a lower level, like regular Poison for Poison 2
				bool upgradedOrSameLevel = parameters.ConditionModel.UpgradableLevel <= ConditionModel.UpgradableLevel;
				return ConditionModel.Stackable || upgradedOrSameLevel;
			},
			async parameters =>
			{
				if(ConditionModel.Stackable)
				{
					parameters.SetAddStack();
				}
				else
				{
					parameters.SetPrevented(true);
				}

				if(parameters.Target.TakingTurn)
				{
					_appliedDuringThisTurn = true;
				}

				await GDTask.CompletedTask;
			}
		);

		if(ConditionModel.RemovedAtEndOfTurn)
		{
			ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.Subscribe(_subscriberPair,
				parameters => parameters.Figure == Owner,
				async parameters =>
				{
					if(ConditionModel.Stackable)
					{
						if(StackCount == 1)
						{
							await AbilityCmd.RemoveCondition(this);
						}
						else
						{
							AdjustStackCount(-1);
						}
					}
					else
					{
						if(_appliedDuringThisTurn)
						{
							_appliedDuringThisTurn = false;
						}
						else
						{
							await AbilityCmd.RemoveCondition(this);
						}
					}
				}
			);
		}

		// Remove any condition that is a lower level of this condition
		for(int i = Owner.Conditions.Count - 1; i >= 0; i--)
		{
			Condition condition = Owner.Conditions[i];
			if(condition.ConditionModel.BaseLevelCondition == ConditionModel.BaseLevelCondition && condition != this)
			{
				await AbilityCmd.RemoveCondition(condition);
			}
		}

		await ConditionModel.OnAdded(this);

		// Conditions like Bless and Curse are removed immediately after being added to a figure
		if(ConditionModel.ImmediatelyRemovedOnApply)
		{
			await AbilityCmd.RemoveCondition(this);
		}
	}

	public async GDTask OnRemoved()
	{
		if(EffectView != null)
		{
			Owner.RemoveEffectView(EffectView);
		}

		await ConditionModel.OnRemoved(this);

		ScenarioEvents.InflictConditionDuplicatesCheckEvent.Unsubscribe(_subscriberPair);
		ScenarioEvents.FigureTurnEndedConditionsFallOffEvent.Unsubscribe(_subscriberPair);
	}

	public void AdjustStackCount(int amount)
	{
		StackCount += amount;
		EffectView?.SetStackCount(StackCount);
		Flash();
	}

	public void Flash()
	{
		EffectView?.Flash();
	}

	public void SetCustomValue(string key, object value)
	{
		_customValues[key] = value;
	}

	public T GetCustomValue<T>(string key)
	{
		if(!_customValues.TryGetValue(key, out object value))
		{
			//Log.Error($"Could not find custom value for key: {key}");
			return default;
		}

		if(value is not T castValue)
		{
			Log.Error($"Could not cast custom value for key: {key}");
			return default;
		}

		return castValue;
	}

	public bool TryGetCustomValue<T>(string key, out T value)
	{
		if(!_customValues.TryGetValue(key, out object retrievedValue))
		{
			value = default;
			return false;
		}

		if(retrievedValue is not T castValue)
		{
			Log.Error($"Could not cast custom value for key: {key}");
			value = default;
			return false;
		}

		value = castValue;
		return true;
	}
}