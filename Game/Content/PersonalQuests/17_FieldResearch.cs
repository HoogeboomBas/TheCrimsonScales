using System.Collections.Generic;
using Fractural.Tasks;
using Newtonsoft.Json;

public class FieldResearch : TheCrimsonScalesPersonalQuest<FieldResearch.Data>
{
	public class Data : PersonalQuestData
	{
		[JsonProperty]
		public List<string> Monsters { get; private set; } = new List<string>();
	}

	public override string Name => "Field Research";
	public override ClassModel ClassToUnlock => ModelDB.Class<MirefootModel>();
	public override int MaxProgress => 20;
	protected override int AtlasIndex => 17;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, Data personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.InflictConditionEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialConditionGiver == character &&
				parameters.ConditionModel is Poison &&
				parameters.Target is Monster monster &&
				!personalQuestData.Monsters.Contains(monster.MonsterModel.Id.ToString()),
			async parameters =>
			{
				personalQuestData.Monsters.Add(((Monster)parameters.Target).MonsterModel.Id.ToString());

				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}