// using System;
// using Fractural.Tasks;
//
// public class Scenario044 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario044.tscn";
// 	public override int ScenarioNumber => 44;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();
//
// 	protected override ScenarioGoals CreateScenarioGoals() =>
// 		new CustomScenarioGoals("");
//
// 	private int _remainingLivingSpiritKills;
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		await AbilityCmd.AddConditions(null, character, [Conditions.Curse, Conditions.Curse, Conditions.Curse]);
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ConcussionMine>());
// 		GameController.Instance.Map.Treasures[1].SetItemLoot(AbilityCmd.GetRandomAvailableStone());
// 		GameController.Instance.Map.Treasures[2].SetItemLoot(ModelDB.Item<DrainingGreaves>());
//
// 		_remainingLivingSpiritKills = GameController.Instance.SavedCampaign.Characters.Count * 2;
// 		UpdateScenarioText(); 
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => _remainingLivingSpiritKills == 0,
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			}
// 		);
//
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this,
// 			parameters => parameters.Figure is Monster monster && monster.MonsterModel == ModelDB.Monster<LivingSpirit>() && _remainingLivingSpiritKills > 0,
// 			async parameters =>
// 			{
// 				
// 				_remainingLivingSpiritKills--;
// 				UpdateScenarioText();
// 				await GDTask.CompletedTask;
// 			}
// 		);
// 	}
//
// 	private void UpdateScenarioText()
//     {
//         GameController.Instance.SpecialRulesView.SetText($"Kill {_remainingLivingSpiritKills} more Living Spirits to win this scenario.");
//     }
// }

