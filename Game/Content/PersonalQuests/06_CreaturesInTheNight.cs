using Fractural.Tasks;

public class CreaturesInTheNight : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Creatures in the Night";
	public override ClassModel ClassToUnlock => ModelDB.Class<ChieftainModel>();
	public override int MaxProgress => 20;
	protected override int AtlasIndex => 6;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialKiller == character &&
				parameters.Figure is Monster monster &&
				(monster.MonsterModel == ModelDB.Monster<Ooze>() ||
				 monster.MonsterModel == ModelDB.Monster<BloodOoze>() ||
				 monster.MonsterModel == ModelDB.Monster<ForestImp>() ||
				 monster.MonsterModel == ModelDB.Monster<ToxicImp>() ||
				 monster.MonsterModel == ModelDB.Monster<BlackImp>()),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}