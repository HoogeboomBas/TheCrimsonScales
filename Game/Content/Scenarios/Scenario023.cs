// using System.Collections.Generic;
// using Fractural.Tasks;
// using System.Linq;
//
// public class Scenario023 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario023.tscn";
// 	public override int ScenarioNumber => 23;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();
// 	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario026>(true)];
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();
//
// 	public override string BGSPath => "res://Audio/BGS/Cave.ogg";
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<IronSnare>());
//
// 		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
// 			parameters =>
// 				!parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
// 					.Any(figure =>
// 						figure.HasCondition(Conditions.Chill) &&
// 						((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)),
// 			async parameters =>
// 			{
// 				parameters.ForgoAction();
//
// 				ActionState actionState = new ActionState(parameters.Performer,
// 				[
// 					OtherAbility.Builder()
// 						.WithPerformAbility(async state =>
// 						{
// 							Figure figure = await AbilityCmd.SelectFigure(state, list =>
// 							{
// 								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 2)
// 									.Where(figure =>
// 										(figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure));
// 							});
//
// 							if(figure == null)
// 							{
// 								return;
// 							}
//
// 							await AbilityCmd.RemoveAllChill(figure);
// 						})
// 						.Build()
// 				]);
// 				await actionState.Perform();
// 			},
// 			EffectType.Selectable,
// 			effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Chill)),
// 			effectInfoViewParameters: new TextEffectInfoView.Parameters(
// 				$"Remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one of your summons within {Icons.Inline(Icons.Range)} 2.")
// 		);
// 	}
// }

