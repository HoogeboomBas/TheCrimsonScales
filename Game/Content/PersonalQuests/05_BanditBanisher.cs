// using Fractural.Tasks;
//
// public class BanditBanisher : TheCrimsonScalesPersonalQuest<PersonalQuestData>
// {
// 	public override string Name => "Bandit Banisher";
// 	public override ClassModel ClassToUnlock => ModelDB.Class<ChainguardModel>();
// 	public override int MaxProgress => 10;
// 	public override ScenarioModel UnlockedScenarioModel => ModelDB.Scenario<Scenario035>();
// 	public override ScenarioModel RequiredCompletedScenario => ModelDB.Scenario<Scenario036>();
// 	protected override int AtlasIndex => 5;
//
// 	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
// 	{
// 		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
// 			parameters =>
// 				parameters.PotentialKiller == character &&
// 				parameters.Figure is Monster monster &&
// 				(monster.MonsterModel == ModelDB.Monster<InoxGuard>() ||
// 				 monster.MonsterModel == ModelDB.Monster<BanditGuard>() ||
// 				 monster.MonsterModel == ModelDB.Monster<CityGuard>() ||
// 				 monster.MonsterModel == ModelDB.Monster<InoxArcher>() ||
// 				 monster.MonsterModel == ModelDB.Monster<BanditArcher>() ||
// 				 monster.MonsterModel == ModelDB.Monster<CityArcher>()),
// 			async parameters =>
// 			{
// 				personalQuestData.AdjustProgress(1, character);
//
// 				await GDTask.CompletedTask;
// 			}
// 		);
// 	}
// }

