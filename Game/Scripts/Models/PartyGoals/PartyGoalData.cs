using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class PartyGoalData
{
	[JsonProperty]
	public int Progress { get; private set; }

	public event Action<PartyGoalData> ProgressChangedEvent;

	public void AdjustProgress(int value)
	{
		SetProgress(Progress + value);
	}

	public void SetProgress(int value)
	{
		if(Progress == value)
		{
			return;
		}

		Progress = value;

		if(GameController.Instance == null || !GameController.FastForward)
		{
			//TODO: Implement
			//AppController.Instance!.PartyGoalProgressUpdateView.AddItem(classModel, personalQuestModel, this);
		}

		FireProgressChangedEvent();
	}

	protected void FireProgressChangedEvent()
	{
		ProgressChangedEvent?.Invoke(this);
	}
}