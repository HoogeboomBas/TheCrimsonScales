using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario017 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario017.tscn";

	public override int ScenarioNumber => 17;
	public override string Name => "Orb Extraction";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario018>(true)];

	public override string IntroductionText =>
		"""
		Delighted with your find, you rush back to the Sanctuary, sure that Athan Tredan will be excited to see your find, and keen to learn about the perilous task you undertook to recover the book for him. You knock on his door and, barely waiting for an answer, you eagerly burst in. Big mistake.

		“Excuse me!” calls out Athan with outrage in his elderly voice “What in the golden meadow is this intrusion? Has all common courtesy left the people of Gloomhaven?!” You are tempted to reply honestly (“generally, yes”), but you feel like naughty children being scolded, and are only able to mumble, “sorry, sir.”

		Athan moves away from the giant diagram he has been studying, which looks like some kind of astrological chart and, still quite grumpily, responds “Well, you have disturbed me now. What do you require of me?”

		You remind him of your previous conversation, and the mission to get the book he requested, to which he replies, “Ah yes, you’re those damn fool adventurers. Well, I did say I wouldn’t have time to help you.”

		This is definitely NOT what he said, but you feel he would not take kindly to an argument, so you merely produce the book you found. His eyes widen and he mumbles, almost to himself, “Well, well, well. The Book of Naiqa.”

		“This is a tremendously useful manuscript and should help both you and my colleagues in regulating the natural order of life. In turn this will allow the Sanctuary to provide greater assistance to the needy of Gloomhaven—there are so many who need our help at this time, you know.” He shakes his head sadly, and you take advantage of the sudden melting of Athan’s grumpy exterior to ask directly about the Frosted Crystal, and the Orb of Embers he had mentioned on your previous visit.

		“Well, as I said, I require the information in this book to give you the full picture, but in essence, the Frosted Crystal and the Orb of Embers contain powerful forces which can be harnessed separately, but together their effects multiply considerably. See them as two weights on a set of scales—true balance and harmony will only be achieved if they are both present.”

		“The last known location of the Orb of Embers was here (he points on the unrecognizable chart), close to what you call the Copperneck Mountains.” Seeing the look on your eyes, he adds “Be wary. If it is still there, and it is a big ‘if’, it will be heavily guarded. The frosted crystal will help you—but beware, the two stones will repel each other violently if they are too close to each other. Do not let them touch.”

		You thank him profusely, note the location, and wish him luck with his endeavors before swiftly leaving before you receive another dressing-down.

		Finding the cave entrance, you realize you’re in the right place as the Frozen Crystal starts to vibrate and glow through its wrappings. You also realize there is one way in, and one way out.
		""";

	public override string ConclusionText =>
		"""
		Grabbing the Orb, you feel the incredible heat—even through your gloves. Much like the Frosted Crystal, the Orb vibrates with latent energy and you can feel the power pulsing through you, giving you extra strength.

		However, it has also driven a new ferocity in the remaining creatures. You turn and make for the exit, slaying whatever you can on the way
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<FrostDemon>(),
		ModelDB.Monster<SavvasIceStorm>(),
		ModelDB.Monster<WindDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainProsperityReward(2),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario018>())
	];

	private IEnumerable<Hex> _markerHexes;
	private Character _orbOfEmbersHolder;
	private Character _frostedCrystalHolder;

	private CustomScenarioGoal _goal;
	private ScenarioRule _startOfScenarioRule;
	private ScenarioRule _orbOfEmbersRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new LootGoalTreasuresGoal());
		_goal = await AddGoal(new CustomScenarioGoal(
			textParameters => $"All characters occupy hexes with {Icons.InlineMarker(Marker.Type.a, textParameters)}.",
			onStart: async goal =>
			{
				ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
					parameters => parameters.Figure is Character,
					async parameters =>
					{
						await goal.SetProgress(
							GameController.Instance.CharacterManager.Characters.Count(character => _markerHexes.Contains(character.Hex)));
					}
				);

				await GDTask.CompletedTask;
			},
			hasProgress: true,
			maxProgress: GameController.Instance.CharacterManager.Characters.Count
		));

		_startOfScenarioRule = AddScenarioRule(textParameters =>
			$"At the start of the scenario, nominate one character to carry the Frosted Crystal. This character may not loot the goal treasure tile and gains {Icons.Inline(Icons.Retaliate, textParameters)}1.");
		_orbOfEmbersRule = AddScenarioRule(textParameters =>
			$"The goal treasure tile contains the Orb of Embers. While a character possesses the Orb of Embers, the character adds +1 to all {Icons.Inline(Icons.Attack, textParameters)} and {Icons.Inline(Icons.Move, textParameters)} abilities. If the character who holds the Orb of Embers becomes exhausted, the scenario is immediately lost. If any character becomes exhausted while not occupying a starting hex, the scenario is lost.");

		_markerHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 6).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainGold(character, 20);
			foreach(Trap trap in RangeHelper.GetHexesInRange(character.Hex, 1).SelectMany(hex => hex.GetHexObjectsOfType<Trap>())
				        .Where(trap => trap != null))
			{
				await AbilityCmd.DisarmTrap(trap, character);
			}
		});

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 19).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainCheckmark(character);
			await AbilityCmd.InfuseWildElement(null, character);
		});

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 43).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainXP(character, 10);
			foreach(ItemModel item in character.Items.Where(item => item.ItemState == ItemState.Spent))
			{
				await AbilityCmd.RefreshItem(item);
			}
		});

		GameController.Instance.Map.Treasures.First(treasure => treasure.IsGoal).SetObtainLootFunction(async character =>
		{
			_orbOfEmbersHolder = character;

			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
				parameters => parameters.Performer == character && parameters.AbilityState is AttackAbility.State or MoveAbility.State,
				async parameters =>
				{
					switch(parameters.AbilityState)
					{
						case MoveAbility.State moveAbilityState:
							moveAbilityState.AdjustMoveValue(1);
							break;
						case AttackAbility.State attackAbilityState:
							attackAbilityState.AbilityAdjustAttackValue(1);
							break;
					}

					await GDTask.CompletedTask;
				}
			);

			_orbOfEmbersRule.SetText(textParameters =>
				$"While a character possesses the Orb of Embers, the character adds +1 to all {Icons.Inline(Icons.Attack, textParameters)} and {Icons.Inline(Icons.Move, textParameters)} abilities. If the character who holds the Orb of Embers becomes exhausted, the scenario is immediately lost. If any character becomes exhausted while not occupying a starting hex, the scenario is lost.");

			await GDTask.CompletedTask;
		});

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character &&
				(parameters.Figure == _orbOfEmbersHolder ||
				 !_markerHexes.Contains(parameters.Figure.Hex)),
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure == _orbOfEmbersHolder ||
				parameters.Figure == _frostedCrystalHolder,
			parameters =>
			{
				if(parameters.Figure == _orbOfEmbersHolder)
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This character is holding the Orb of Embers."));
				}
				else
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This character is holding the Frosted Crystal."));
				}
			}
		);
	}

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		_frostedCrystalHolder = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to hold the Frosted Crystal") as Character;

		ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.Figure == _frostedCrystalHolder,
			applyParameters =>
			{
				applyParameters.AddRetaliate(1, 1);
			}
		);

		ScenarioEvents.RetaliateEvent.Subscribe(this,
			canApplyParameters =>
				canApplyParameters.RetaliatingFigure == _frostedCrystalHolder &&
				RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, _frostedCrystalHolder.Hex) <= 1,
			async applyParameters =>
			{
				applyParameters.AdjustRetaliate(1);

				await GDTask.CompletedTask;
			}
		);

		await _goal.SetProgress(
			GameController.Instance.CharacterManager.Characters.Count(character => _markerHexes.Contains(character.Hex)));

		_startOfScenarioRule.SetText(textParameters =>
			$"The character holding the Frosted Crystal may not loot the goal treasure tile and gains {Icons.Inline(Icons.Retaliate, textParameters)}1.");

		Treasure goalTreasure = GameController.Instance.Map.Treasures.First(treasure => treasure.IsGoal);
		goalTreasure.SetCanLootFunction(figure => figure != _frostedCrystalHolder);
	}
}