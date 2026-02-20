// using Fractural.Tasks;
//
// public class ProtectAndServe : TheCrimsonScalesPersonalQuest<PersonalQuestData>
// {
// 	public override string Name => "Protect and Serve";
// 	public override ClassModel ClassToUnlock => ModelDB.Class<BombardModel>();
// 	public override int MaxProgress => 10;
// 	public override ScenarioModel UnlockedScenarioModel => ModelDB.Scenario<Scenario033>();
// 	public override ScenarioModel RequiredCompletedScenario => ModelDB.Scenario<Scenario034>();
// 	protected override int AtlasIndex => 0;
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
// 				 monster.MonsterModel == ModelDB.Monster<InoxArcher>() ||
// 				 monster.MonsterModel == ModelDB.Monster<InoxShaman>()),
// 			async parameters =>
// 			{
// 				personalQuestData.AdjustProgress(1, character);
//
// 				await GDTask.CompletedTask;
// 			}
// 		);
// 	}
// }

