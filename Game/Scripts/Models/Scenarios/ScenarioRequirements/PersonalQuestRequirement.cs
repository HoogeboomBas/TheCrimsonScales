using System.Linq;

public class PersonalQuestRequirement : ScenarioRequirement
{
	private readonly PersonalQuestModel _personalQuestModel;

	public PersonalQuestRequirement(PersonalQuestModel personalQuestModel)
	{
		_personalQuestModel = personalQuestModel;
	}

	public override bool GetMet(SavedCampaign savedCampaign)
	{
		return savedCampaign.Characters.Any(character => character.SavedPersonalQuest.Model == _personalQuestModel);
	}

	public override string NotMetMessage()
	{
		return $"You require a character with the {_personalQuestModel.Name} personal quest.";
	}
}