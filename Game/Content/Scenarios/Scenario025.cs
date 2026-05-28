using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario025 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario025.tscn";

	public override int ScenarioNumber => 25;
	public override string Name => "Brightspark, Behold!";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<WondrousScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario031>()];

	public override string IntroductionText =>
		"""
		The Aesther’s notes you recovered from the Den of Monstrosity mentioned another master of elements. Unsure what to think, but always keen for an adventure, you travel to the location you have for him, holed up in the old Harbour Master’s office.

		As you enter the building, you are greeted by a human figure in a long coat you have never seen before.

		“Ah, friends!” he calls in greeting “Right on time as usual!” You exchange glances at this curious comment, before he continues at break-neck speed. “I have a little bother here—nothing we can’t sort between us though I’m sure! I store my ability flasks in here for safe-keeping, as I’m sure you know, but I need a little help in reaching them. One or two little scamps have found a way in and I can’t reach them! Will you do a chap a favor and lend a hand?”

		Deeply confused by this strange man and his ‘ability flasks’ you look puzzled, allowing him to jump back in.

		“Of course! We haven’t met in this dimension yet, how foolish!” he cries letting out an enormous laugh, which again completely passes you by. “I am the Brightspark! I know you though, so let’s just say there will be ample reward to suit lords and ladies of your ability and hunger. Right, let’s go! I may be short of my abilities, but I can teach these interlopers a thing or two!”

		With that he leaps into battle. Deeply confused, but feeling that he probably needs saving from himself, especially if he is to pay you, you follow him into the fight. “Rescue my flasks will you, my good fellows? They’re in the chests. Onwards!”
		""";

	public override string ConclusionText =>
		"""
		“I say!” exclaims the Brightspark “That was the best fun I’ve had in ages! You are the most extraordinary group of pugilists! If you ever need me, and if you deem me worthy—please don’t hesitate to call on me.”

		As he turns to leave, he calls—“Oh, and watch the Eternal Demon—he’s a tricky fellow.”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<ForestImp>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<SpittingDrake>(),
		ModelDB.Monster<SunDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(1),
		new AddRoadReward(ModelDB.Event<Road57>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario031>())
	];

	private int _treasuresLooted;
	private Hex _markerAHex;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new LootGoalTreasuresGoal(requiredTreasureCount: 4));

// 		string text =
// 			$"""
// 			 Loot {4 - _treasuresLooted} more Goal treasure tiles and keep the Brightspark alive to win this scenario.
//
// 			 The Brightspark acts on Initiative 50 every turn, performing “{Icons.Inline(Icons.Move)}2, {Icons.Inline(Icons.Attack)}1” (using whichever modifier deck you prefer). For each Goal treasure tile you loot, the Brightspark gains the following benefit:
// 			 First tile: Add +1{Icons.Inline(Icons.Move)} to all moves
// 			 Second tile: Add +1{Icons.Inline(Icons.Attack)} to all attacks
// 			 Third tile: Add {Icons.Inline(Icons.Jump)} to all moves
//
// 			 Additionally, the Brightspark can consume {Icons.Inline(Icons.WildElement)} at the end of its turn to perform {Icons.Inline(Icons.Heal)}2, self. This is optional and players determine if this is performed.
//
// 			 Whenever there are no monsters on the map, the Brightspark will move toward hex {Icons.InlineMarker(Marker.Type.a)}.
//
// 			 If the Brightspark is killed, the scenario is immediately lost.
// 			 """;

		AddScenarioRule("The Brightspark is an ally to you and an enemy to all monsters.");
		AddScenarioRule("The Brightspark draws from whichever modifier deck you prefer.");
		AddScenarioRule("If the Brightspark is killed, the scenario is lost.");

		//The Brightspark acts on Initiative 50 every turn, performing “{Icons.Inline(Icons.Move, textParameters)}2, {Icons.Inline(Icons.Attack, textParameters)}1” (using whichever modifier deck you prefer). 
		AddScenarioRule(textParameters =>
			$"""
			 For each Goal treasure tile you loot, the Brightspark gains the following benefit:
			 First tile: Add +1{Icons.Inline(Icons.Move, textParameters)} to all moves.
			 Second tile: Add +1{Icons.Inline(Icons.Attack, textParameters)} to all attacks.
			 Third tile: Add {Icons.Inline(Icons.Jump, textParameters)} to all moves.
			 """);

		AddScenarioRule(textParameters =>
			$"Additionally, the Brightspark can consume {Icons.Inline(Icons.WildElement, textParameters)} at the end of its turn to perform {Icons.Inline(Icons.Heal, textParameters)}2, self. This is optional and players determine if this is performed.");

		AddScenarioRule(textParameters =>
			$"Whenever there are no monsters on the map, the Brightspark will move toward hex {Icons.InlineMarker(Marker.Type.a, textParameters)}.");

		_markerAHex = GameController.Instance.Map.GetMarker(Marker.Type.a).Hex;

		NPC brightspark = await SpawnNPC(GameController.Instance.Map.GetMarker(Marker.Type.b).Hex, CharacterCount + ScenarioLevel * 3, "Brightspark",
			"res://Content/Scenarios/NPCs/Brightspark", 50, [
				MoveAbility.Builder().WithDistance(2).Build(),
				AttackAbility.Builder().WithDamage(1).Build()
			], textParameters => $"{Icons.Inline(Icons.Move, textParameters)}2\n{Icons.Inline(Icons.Attack, textParameters)}1");

		foreach(Treasure treasure in GameController.Instance.Map.Treasures)
		{
			treasure.SetObtainLootFunction(async _ =>
			{
				switch(_treasuresLooted)
				{
					case 0:
						ScenarioEvents.AbilityStartedEvent.Subscribe(this, treasure,
							parameters => parameters.AbilityState is MoveAbility.State && parameters.Performer == brightspark,
							async parameters =>
							{
								((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(1);
								await GDTask.CompletedTask;
							});
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, treasure,
							parameters => parameters.Figure == brightspark,
							parameters =>
							{
								parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
									$"Add +1{Icons.Inline(Icons.Move, textParameters)} to all moves."));
							});
						break;
					case 1:
						ScenarioEvents.AbilityStartedEvent.Subscribe(this, treasure,
							parameters => parameters.AbilityState is AttackAbility.State && parameters.Performer == brightspark,
							async parameters =>
							{
								((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(1);
								await GDTask.CompletedTask;
							});
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, treasure,
							parameters => parameters.Figure == brightspark,
							parameters =>
							{
								parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
									$"Add +1{Icons.Inline(Icons.Attack, textParameters)} to all attacks."));
							});
						break;
					case 2:
						ScenarioEvents.AbilityStartedEvent.Subscribe(this, treasure,
							parameters => parameters.AbilityState is MoveAbility.State && parameters.Performer == brightspark,
							async parameters =>
							{
								((MoveAbility.State)parameters.AbilityState).AddJump();
								await GDTask.CompletedTask;
							});
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this, treasure,
							parameters => parameters.Figure == brightspark,
							parameters =>
							{
								parameters.Add(new InfoTextExtraEffect.Parameters(textParameters =>
									$"Add {Icons.Inline(Icons.Jump, textParameters)} to all moves."));
							});
						break;
				}

				_treasuresLooted++;

				await GDTask.CompletedTask;
			});
		}

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure == brightspark,
			async _ =>
			{
				await AbilityCmd.Lose();
			}
		);

		ScenarioEvents.FigureTurnEndingEvent.Subscribe(this,
			ScenarioEvents.FigureTurnEnding.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
				canApplyFunction: applyParameters => applyParameters.Figure == brightspark,
				applyFunction: async applyParameters =>
				{
					await new ActionState(brightspark, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]).Perform();
				}, EffectType.Selectable,
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}2, self"))
		);

		ScenarioEvents.FigureFoundFocusEvent.Subscribe(this,
			parameters =>
				parameters.Performer == brightspark &&
				parameters.AbilityState is MoveAbility.State &&
				parameters.Focus == null,
			async parameters =>
			{
				parameters.SetFocusHex(_markerAHex);

				ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(this,
					moveParameters => moveParameters.Performer == brightspark,
					moveParameters =>
					{
						moveParameters.SetRange(0);
						moveParameters.SetRangeType(RangeType.Melee);
						moveParameters.SetTargets(1);
						moveParameters.SetAOEPattern(null);
					}
				);

				ScenarioEvents.AbilityEndedEvent.Subscribe(this,
					abilityEndedParameters => abilityEndedParameters.Performer == brightspark,
					async _ =>
					{
						ScenarioEvents.AbilityEndedEvent.Unsubscribe(this);
						ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(this);

						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			}
		);

		bool changedDeck = false;
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this, character,
				parameters => parameters.Performer == brightspark && !character.IsDead && !changedDeck,
				async _ =>
				{
					changedDeck = true;
					brightspark.SetAMDCardDeck(character.AMDCardDeck);

					await GDTask.CompletedTask;
				}, EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(character.ClassModel.IconPath),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"Use {character.SavedCharacter.GetNameAndIcon()}'s attack modifier deck.")
			);

			ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, character,
				parameters => parameters.Performer == brightspark,
				async parameters =>
				{
					changedDeck = false;

					await GDTask.CompletedTask;
				}
			);
		}
	}
}