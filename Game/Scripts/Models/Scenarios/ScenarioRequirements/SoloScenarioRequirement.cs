using System.Linq;

public class SoloScenarioRequirement : ScenarioRequirement
{
	private readonly ClassModel _classModel;

	public SoloScenarioRequirement(ClassModel classModel)
	{
		_classModel = classModel;
	}

	public override bool GetMet(SavedCampaign savedCampaign)
	{
		SavedCharacter savedCharacter = savedCampaign.Characters.FirstOrDefault(character => character.ClassModel == _classModel);
		return savedCharacter != null && savedCharacter.Level >= 5 && !savedCharacter.SoloScenarioCompleted;
	}

	public override string NotMetMessage(SavedCampaign savedCampaign)
	{
		SavedCharacter savedCharacter = savedCampaign.Characters.FirstOrDefault(character => character.ClassModel == _classModel);
		return savedCharacter == null
			? $"You require a {_classModel.Name} character to play its solo scenario"
			: savedCharacter.SoloScenarioCompleted
				? $"{_classModel.Name} has already completed their solo scenario."
				: $"You require {_classModel.Name} to be at least level 5 to play its solo scenario";
	}
}