using Fractural.Tasks;
using Newtonsoft.Json;

public abstract class PartyGoalModel<T> : PartyGoalModel
	where T : PartyGoalData, new()
{
	public sealed override PartyGoalData CreateData()
	{
		return new T();
	}

	public sealed override int GetProgress(SavedPartyGoal savedPartyGoal)
	{
		return GetProgress((T)savedPartyGoal.PartyGoalData);
	}

	protected virtual int GetProgress(T partyGoalData)
	{
		return partyGoalData.Progress;
	}

	protected sealed override void SubscribeDuringDowntime(SavedPartyGoal savedPartyGoal)
	{
		SubscribeDuringDowntime((T)savedPartyGoal.PartyGoalData);
	}

	protected sealed override void UnsubscribeDuringDowntime(SavedPartyGoal savedPartyGoal)
	{
		UnsubscribeDuringDowntime((T)savedPartyGoal.PartyGoalData);
	}

	public sealed override async GDTask OnScenarioSetupPhaseCompleted(SavedPartyGoal savedPartyGoal)
	{
		// Clone the party goal data to overwrite the original later, after the scenario is finished
		string serializedData = JsonConvert.SerializeObject(savedPartyGoal.PartyGoalData, SaveManager.JsonSerializerSettings);
		T clonedGoalData = JsonConvert.DeserializeObject<T>(serializedData);

		await OnScenarioSetupPhaseCompleted(clonedGoalData);

		GameController.Instance.EndEvent += OnEndEvent;
		return;

		void OnEndEvent(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
		{
			savedPartyGoal.OverwritePartyGoalData(clonedGoalData);
		}
	}

	protected virtual void SubscribeDuringDowntime(T partyGoalData)
	{
	}

	protected virtual void UnsubscribeDuringDowntime(T partyGoalData)
	{
	}

	protected virtual async GDTask OnScenarioSetupPhaseCompleted(T partyGoalData)
	{
		await GDTask.CompletedTask;
	}
}

public abstract class PartyGoalModel : AbstractModel
{
	public abstract bool ScalesWithCharacterCount { get; }
	public abstract int MaxProgress { get; }

	public abstract string GetText(int characterCount);

	public abstract PartyGoalData CreateData();

	public abstract int GetProgress(SavedPartyGoal savedPartyGoal);

	public virtual async GDTask OnBetweenScenariosStarted(SavedPartyGoal savedPartyGoal)
	{
		SubscribeDuringDowntime(savedPartyGoal);

		await GDTask.CompletedTask;
	}

	protected abstract void SubscribeDuringDowntime(SavedPartyGoal savedPartyGoal);

	public virtual void OnBetweenScenariosEnded(SavedPartyGoal savedPartyGoal)
	{
		UnsubscribeDuringDowntime(savedPartyGoal);
	}

	protected abstract void UnsubscribeDuringDowntime(SavedPartyGoal savedPartyGoal);

	public abstract GDTask OnScenarioSetupPhaseCompleted(SavedPartyGoal savedPartyGoal);
}