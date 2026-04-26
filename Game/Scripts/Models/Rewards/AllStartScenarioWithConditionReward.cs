using System;
using System.Linq;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllStartScenarioWithConditionReward : SavedReward
{
	[JsonProperty]
	private string[] _conditionModelsIds;

	public ConditionModel[] ConditionModels => _conditionModelsIds.Select(ModelDB.GetById<ConditionModel>).ToArray();

	public override RewardType Type => RewardType.ScenarioStart;

	public AllStartScenarioWithConditionReward()
	{
	}

	public AllStartScenarioWithConditionReward(params ConditionModel[] conditionModels)
	{
		_conditionModelsIds = conditionModels.Select(model => model.Id.ToString()).ToArray();
	}

	public override string GetLabelText(RichTextParameters textParameters)
	{
		string labelText = "All characters start the next scenario with ";

		ConditionModel[] conditionModels = ConditionModels;
		for(int i = 0; i < conditionModels.Length; i++)
		{
			ConditionModel conditionModel = conditionModels[i];
			if(i > 0)
			{
				labelText += ", ";
			}

			labelText += Icons.InlineCondition(conditionModel, textParameters);
		}

		labelText += ".";

		return labelText;
	}

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		ConditionModel[] conditionModels = ConditionModels;
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			foreach(ConditionModel conditionModel in conditionModels)
			{
				ScenarioEvents.InflictConditionEventReward.Parameters inflictConditionsParameters =
					await ScenarioEvents.InflictConditionEventRewardEvent.CreatePrompt(
						new ScenarioEvents.InflictConditionEventReward.Parameters(character, conditionModel), character);

				if(!inflictConditionsParameters.Prevented)
				{
					await AbilityCmd.AddCondition(null, character, conditionModel);
				}
			}
		}
	}
}