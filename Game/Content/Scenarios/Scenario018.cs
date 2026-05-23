using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario018 : ScenarioModel
{
	public class Scenario018DowntimeEnhancementCostReward : DowntimeEnhancementCostReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Each character gets {Icons.Inline(Icons.Coins, textParameters)}100 to buy enhancements during the next City Phase.";

		public Scenario018DowntimeEnhancementCostReward()
		{
		}

		protected override void CalculateCostApplyFunction(BetweenScenariosEvents.CalculateEnhancementCost.Parameters parameters)
		{
			int goldLeft = 100 - GetCustomValue<int>(parameters.Buyer.Guid.ToString());
			int costAdjustment = Mathf.Min(parameters.Cost, goldLeft);
			parameters.AdjustCost(-costAdjustment);
		}

		protected override void EnhancementBoughtApplyFunction(BetweenScenariosEvents.EnhancementBought.Parameters parameters)
		{
			int costAdjustment = parameters.BaseCost - parameters.Cost;
			SetCustomValue(parameters.Buyer.Guid.ToString(), int.Min(100, GetCustomValue<int>(parameters.Buyer.Guid.ToString() + costAdjustment)));

			// if(parameters.EnhancementModel is IPlusOneEnhancement && parameters.SavedAbilityCard.Model.Level == 1)
			// {
			// 	Complete();
			// }
		}
	}

	public override string ScenePath => "res://Content/Scenarios/Scenario018.tscn";

	public override int ScenarioNumber => 18;
	public override string Name => "Grab and Go";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario020>(), new ScenarioConnection<Scenario021>()];

	public override string IntroductionText =>
		"""
		You sprint back out of the cave, clutching the Orb of Embers tightly. You hear a roar from the beasts in the cave, who seem to have multiplied, and dash down a tight valley to the right of the cavern.

		You’re confident you can escape this way as long as you’re quick—and nothing that sees you is left alive to give you away.
		""";

	public override string ConclusionText =>
		"""
		As you suspected, the tight corridor opens to a rocky outcrop in the forest. Glad to be seeing daylight once more, you scramble down the rocks, and through a small wood until you pick up a well-travelled track. You head East, in the general direction of Gloomhaven, and you eventually find a larger road you recognize. A couple of hours later, you’re in the Sleeping Lion again, laughing at your daring escape and marveling at your two powerful items, which seem to have increased your individual abilities.

		After a few rowdy drinks, your thoughts eventually turn to Athan and Selandre. Should you go and show Athan your acquisition, or Selandre?
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<HarrowerInfester>(),
		ModelDB.Monster<NightDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new Scenario018DowntimeEnhancementCostReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario020>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario021>()),
	];

	private IEnumerable<Hex> _markerHexes;

	private CustomScenarioGoal _goal;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
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

		AddScenarioRule(textParameters =>
			$"If any character is exhausted while not occupying a hex {Icons.InlineMarker(Marker.Type.a, textParameters)}, the scenario is lost.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<BootsOfPerpetuity>());

		_markerHexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Character &&
				!_markerHexes.Contains(parameters.Figure.Hex),
			async parameters =>
			{
				await AbilityCmd.Lose();
			}
		);
	}

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		await _goal.SetProgress(
			GameController.Instance.CharacterManager.Characters.Count(character => _markerHexes.Contains(character.Hex)));
	}
}