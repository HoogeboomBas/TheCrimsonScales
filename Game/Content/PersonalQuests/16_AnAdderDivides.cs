using System.Collections.Generic;
using Fractural.Tasks;

public class AnAdderDivides : TheCrimsonScalesPersonalQuest<AnAdderDivides.Data>
{
	public class Data : PersonalQuestData
	{
		public List<string> DifficultTerrainScenarios { get; private set; } = new List<string>();
	}

	public override string Name => "An Adder Divides";
	public override ClassModel ClassToUnlock => ModelDB.Class<MirefootModel>();
	public override int MaxProgress => 6;
	public override ScenarioModel UnlockedScenarioModel => ModelDB.Scenario<Scenario039>();
	public override ScenarioModel RequiredCompletedScenario => ModelDB.Scenario<Scenario039>();
	protected override int AtlasIndex => 16;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, Data personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		if(personalQuestData.DifficultTerrainScenarios.Contains(GameController.Instance.ScenarioModel.Id.ToString()))
		{
			return;
		}

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(character, this,
			parameters =>
				parameters.Figure == character &&
				parameters.Hex.HasHexObjectOfType<DifficultTerrain>(),
			async parameters =>
			{
				// Can only add progress once per scenario
				ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(character, this);
				personalQuestData.DifficultTerrainScenarios.Add(GameController.Instance.ScenarioModel.Id.ToString());

				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}