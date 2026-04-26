using System;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedPartyGoal
{
	[JsonProperty]
	public string ModelId { get; private set; }

	[JsonProperty]
	public PartyGoalData PartyGoalData { get; private set; }

	public PartyGoalModel Model => ModelDB.GetById<PartyGoalModel>(ModelId);

	public SavedPartyGoal()
	{
	}

	public SavedPartyGoal(PartyGoalModel model)
	{
		ModelId = model.Id.ToString();
		PartyGoalData = model.CreateData();
	}

	public void OverwritePartyGoalData(PartyGoalData partyGoalData)
	{
		PartyGoalData = partyGoalData;
	}
}