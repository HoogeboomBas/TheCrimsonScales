using Fractural.Tasks;
using Godot;

public class AllMonstersStartScenarioWithConditionEventReward(params ConditionModel[] conditionModels) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor)
	{
		string labelText = "At the start of the next scenario, all visible monsters gain ";

		for(int i = 0; i < conditionModels.Length; i++)
		{
			ConditionModel conditionModel = conditionModels[i];
			if(i > 0)
			{
				labelText += ", ";
			}

			labelText += Icons.Inline(Icons.GetCondition(conditionModel));
		}

		labelText += ".";

		return labelText;
	}

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

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