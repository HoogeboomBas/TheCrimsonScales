using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class PersonalQuestData
{
	[JsonProperty]
	public int Progress { get; private set; }

	public event Action<PersonalQuestData> ProgressChangedEvent;

	public void AdjustProgress(int value, Character character)
	{
		AdjustProgress(value, character.SavedCharacter);
	}

	public void AdjustProgress(int value, SavedCharacter savedCharacter)
	{
		AdjustProgress(value, savedCharacter.ClassModel, savedCharacter.SavedPersonalQuest.Model);
	}

	private void AdjustProgress(int value, ClassModel classModel, PersonalQuestModel personalQuestModel)
	{
		Progress += value;

		if(GameController.Instance == null || !GameController.FastForward)
		{
			AppController.Instance!.PersonalQuestProgressUpdateView.AddItem(classModel, personalQuestModel, this);
		}

		ProgressChangedEvent?.Invoke(this);
	}
}