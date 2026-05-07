using System;
using System.Linq;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllMonstersStartScenarioWithConditionReward : SavedReward
{
	[JsonProperty]
	private string[] _conditionModelsIds;

	public ConditionModel[] ConditionModels => _conditionModelsIds.Select(ModelDB.GetById<ConditionModel>).ToArray();

	public override RewardType Type => RewardType.ScenarioStart;

	public AllMonstersStartScenarioWithConditionReward()
	{
	}

	public AllMonstersStartScenarioWithConditionReward(params ConditionModel[] conditionModels)
	{
		_conditionModelsIds = conditionModels.Select(model => model.Id.ToString()).ToArray();
	}

	public override string GetLabelText(RichTextParameters textParameters)
	{
		string labelText = "At the start of the next scenario, all visible monsters gain ";

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
		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			if(figure is Monster)
			{
				foreach(ConditionModel conditionModel in conditionModels)
				{
					await AbilityCmd.AddCondition(null, figure, conditionModel);
				}
			}
		}
	}
}