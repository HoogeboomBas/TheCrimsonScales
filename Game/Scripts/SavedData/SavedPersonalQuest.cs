using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedPersonalQuest
{
	[JsonProperty]
	public string ModelId { get; private set; }

	[JsonProperty]
	public PersonalQuestData PersonalQuestData { get; private set; }

	public PersonalQuestModel Model => ModelDB.GetById<PersonalQuestModel>(ModelId);

	public SavedPersonalQuest()
	{
	}

	public SavedPersonalQuest(PersonalQuestModel model)
	{
		ModelId = model.Id.ToString();
		PersonalQuestData = model.CreateData();
	}

	public void OverwritePersonalQuestData(PersonalQuestData personalQuestData)
	{
		PersonalQuestData = personalQuestData;
	}
}