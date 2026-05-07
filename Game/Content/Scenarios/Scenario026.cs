// using System.Collections.Generic;
// using System.Linq;
// using Fractural.Tasks;
// using Godot;
//
// public class Scenario026 : ScenarioModel
// {
// 	public override string ScenePath => "res://Content/Scenarios/Scenario026.tscn";
// 	public override int ScenarioNumber => 26;
// 	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<ChillyScenarioChain>();
// 	private int _thermalStonesDestroyed = 0;
//
// 	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals("");
//
// 	private string _text;
//
// 	public override string BGSPath => "res://Audio/BGS/Cave.ogg";
//
// 	public override async GDTask StartOfScenarioEffects(Character character)
// 	{
// 		await AbilityCmd.AddCondition(null, character, Conditions.Chill);
// 	}
//
// 	public override async GDTask InitializeAfterFirstRoomRevealed()
// 	{
// 		await base.InitializeAfterFirstRoomRevealed();
//
// 		List<Objective> coldThermalStones = GameController.Instance.Map.Rooms[0].GetChildrenOfType<Objective>();
// 		List<Objective> hotThermalStones = GameController.Instance.Map.Rooms[1].GetChildrenOfType<Objective>();
// 		Objective icyFireThermalStone = GameController.Instance.Map.Rooms[2].GetChildrenOfType<Objective>()[0];
// 		int thermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count + 3;
// 		int icyFireThermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count * 6;
//
// 		foreach(Objective objective in coldThermalStones)
// 		{
// 			objective.Init(thermalStoneHealth, "Cold Thermal Stone");
// 			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, objective,
// 				canApplyParameters => canApplyParameters.AbilityState.Target == objective &&
// 				                      (canApplyParameters.AbilityState.Performer is Character || canApplyParameters.AbilityState.Performer is Summon),
// 				async applyParameters =>
// 				{
// 					await AbilityCmd.AddCondition(null, applyParameters.AbilityState.Performer, Conditions.Chill);
// 				}
// 			);
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this, objective,
// 				canApplyParameters => canApplyParameters.Figure == objective && (canApplyParameters.PotentialAbilityState.Performer is Character ||
// 				                                                                 canApplyParameters.PotentialAbilityState.Performer is Summon),
// 				async applyParameters =>
// 				{
// 					_thermalStonesDestroyed++;
// 					UpdateScenarioText(_text);
// 					await AbilityCmd.RemoveAllChill(applyParameters.PotentialAbilityState.Performer);
// 					await AbilityCmd.CreateDifficultTerrain(objective.Hex,
// 						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn"));
// 					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, objective);
// 					ScenarioEvents.FigureKilledEvent.Unsubscribe(this, objective);
// 				}
// 			);
// 		}
//
// 		foreach(Objective objective in hotThermalStones)
// 		{
// 			objective.Init(thermalStoneHealth, "Hot Thermal Stone");
// 			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, objective,
// 				canApplyParameters => canApplyParameters.AbilityState.Target == objective &&
// 				                      (canApplyParameters.AbilityState.Performer is Character || canApplyParameters.AbilityState.Performer is Summon),
// 				async applyParameters =>
// 				{
// 					await AbilityCmd.SufferDamage(applyParameters.AbilityState.Performer, 1, objective);
// 				}
// 			);
// 			ScenarioEvents.FigureKilledEvent.Subscribe(this, objective,
// 				canApplyParameters => canApplyParameters.Figure == objective && (canApplyParameters.PotentialAbilityState.Performer is Character ||
// 				                                                                 canApplyParameters.PotentialAbilityState.Performer is Summon),
// 				async applyParameters =>
// 				{
// 					_thermalStonesDestroyed++;
// 					UpdateScenarioText(_text);
// 					HealAbility heal = HealAbility.Builder()
// 						.WithHealValue(3)
// 						.WithTarget(Target.Self)
// 						.Build();
// 					ActionState actionState = new ActionState(applyParameters.Figure, [heal]);
// 					await actionState.Perform();
// 					await AbilityCmd.CreateOverlayTile<HazardousTerrain>(objective.Hex,
// 						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/HazardousTerrain/HotCoals1H.tscn"));
// 					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, objective);
// 					ScenarioEvents.FigureKilledEvent.Unsubscribe(this, objective);
// 				}
// 			);
// 		}
//
// 		icyFireThermalStone.Init(icyFireThermalStoneHealth, "Icy Fire Thermal Stone");
// 		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, icyFireThermalStone,
// 			canApplyParameters => canApplyParameters.AbilityState.Target == icyFireThermalStone &&
// 			                      (canApplyParameters.AbilityState.Performer is Character || canApplyParameters.AbilityState.Performer is Summon),
// 			async applyParameters =>
// 			{
// 				await AbilityCmd.AddConditions(null, applyParameters.AbilityState.Performer, [Conditions.Chill, Conditions.Wound1]);
// 			}
// 		);
// 		ScenarioEvents.FigureKilledEvent.Subscribe(this, icyFireThermalStone,
// 			canApplyParameters => canApplyParameters.Figure == icyFireThermalStone &&
// 			                      (canApplyParameters.PotentialAbilityState.Performer is Character ||
// 			                       canApplyParameters.PotentialAbilityState.Performer is Summon),
// 			async applyParameters =>
// 			{
// 				_thermalStonesDestroyed++;
// 				UpdateScenarioText(_text);
// 				Figure figure = applyParameters.PotentialAbilityState.Performer;
// 				await AbilityCmd.RemoveAllNegativeConditions(figure);
// 				ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, icyFireThermalStone);
// 				ScenarioEvents.FigureKilledEvent.Unsubscribe(this, icyFireThermalStone);
// 			}
// 		);
//
// 		_text = $"""
// 		         Each boulder on L3B represents a Cold thermal stone and has {thermalStoneHealth} hit points. Each time a character or character summons attacks a Cold thermal stone, they gain {Icons.Inline(Icons.GetCondition(Conditions.Chill))} immediately following the attack.
//
// 		         When a character or character summon destroys a Cold thermal stone, they immediately remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self and place a water tile in the hex it was occupying.
// 		         """;
// 		UpdateScenarioText(_text);
//
// 		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<OrbOfDespair>());
//
// 		ScenarioEvents.RoundEndedEvent.Subscribe(this,
// 			parameters => _thermalStonesDestroyed == 5,
// 			async parameters =>
// 			{
// 				await ((CustomScenarioGoals)ScenarioGoals).Win();
// 			}
// 		);
//
// 		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
// 			parameters => !parameters.ForgoneAction && RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 2)
// 				.Any(figure => figure.HasCondition(Conditions.Chill) &&
// 				               ((figure is Summon summon && summon.Owner == parameters.Performer) || parameters.Performer == figure)),
// 			async parameters =>
// 			{
// 				parameters.ForgoAction();
//
// 				ActionState actionState = new ActionState(parameters.Performer, [
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
//
// 	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
// 	{
// 		await base.OnRoomRevealed(parameters);
//
// 		if(parameters.Room == GameController.Instance.Map.Rooms[1])
// 		{
// 			int thermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count + 3;
// 			_text = $"""
// 			         Each boulder on G2A represents a Hot thermal stone and has {thermalStoneHealth} hit points. Each time a character or character summon attacks a Hot thermal stone, they immediately suffer {Icons.Inline(Icons.Damage)} 1 following the attack.
//
// 			         When a character or character summon destroys a Hot thermal stone, they immediately perform {Icons.Inline(Icons.Heal)}3, Self and place a hot coal tile in the hex it was occupying.
// 			         """;
// 			UpdateScenarioText(_text);
// 		}
// 		else if(parameters.Room == GameController.Instance.Map.Rooms[2])
// 		{
// 			int icyFireThermalStoneHealth = GameController.Instance.SavedCampaign.Characters.Count * 6;
// 			_text = $"""
// 			         The boulder marked represents the Icy Flame thermal stone and has {icyFireThermalStoneHealth} hit points. Each time a character or character summonattacks an Icy Flame thermal stone, they immediately gain {Icons.Inline(Icons.GetCondition(Conditions.Wound1))} and {Icons.Inline(Icons.GetCondition(Conditions.Chill))}.
//
// 			         When a character or character summon destroys the Icy Flame thermal stone, they immediately remove all negative conditions from self.
// 			         """;
// 			UpdateScenarioText(_text);
// 		}
// 	}
//
// 	protected override void UpdateScenarioText(string text)
// 	{
// 		string displayText = $"""
// 		                      Destroy {5 - _thermalStonesDestroyed} more Thermal Stones to win this scenario.
//
// 		                      Any character may forgo the top or bottom action of their turn to remove all {Icons.Inline(Icons.GetCondition(Conditions.Chill))} from self or one summon they own within {Icons.Inline(Icons.Range)} 2.
//
//
// 		                      """ + text;
// 		GameController.Instance.SpecialRulesView.SetText(displayText);
// 	}
// }

