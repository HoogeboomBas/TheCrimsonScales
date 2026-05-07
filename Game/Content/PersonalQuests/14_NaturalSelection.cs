using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class NaturalSelection : TheCrimsonScalesPersonalQuest<NaturalSelection.Data>
{
	public class Data : PersonalQuestData
	{
		public List<Element> Elements { get; private set; } = new List<Element>();
	}

	public override string Name => "Natural Selection";
	public override ClassModel ClassToUnlock => ModelDB.Class<LuminaryModel>();
	public override int MaxProgress => 8;
	public override ScenarioModel UnlockedScenarioModel => ModelDB.Scenario<Scenario037>();
	public override ScenarioModel RequiredCompletedScenario => ModelDB.Scenario<Scenario038>();
	protected override int AtlasIndex => 14;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, Data personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.ElementInfusedEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialInfuser == character &&
				parameters.Element is Element.Fire or Element.Ice or Element.Light or Element.Dark &&
				personalQuestData.Elements.Count(element => element == parameters.Element) < 2,
			async parameters =>
			{
				personalQuestData.Elements.Add(parameters.Element);

				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}

