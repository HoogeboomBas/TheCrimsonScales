using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario046 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario046.tscn";

	public override int ScenarioNumber => 46;
	public override string Name => "Vermling Gardens";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Led by the instructions of the dying Vermling you discovered by the roadside, you find the fabled Vermling Gardens.

		Inside the ancient walled garden, there are various trees and bushes that bear magical fruit. However as the Vermling told you with his dying breath, deep within the Gardens is a tree whose fruit does not heal those who eat it, but instead is highly poisonous, and usually fatal.

		Cutting down the tree would be a fairly straightforward task, except that the Vermlings jealously protect the Gardens from other races and are unimpressed to see a scruffy group of mercenaries approaching their sacred space.

		It seems slightly unfortunate, but in order to save the Vermlings, it looks like you’re going to have to kill a few.
		""";

	public override string ConclusionText =>
		"""
		After much scuffling, the Vermlings are subdued (some permanently) to the point where you can cut down the tree.

		As you hack the tree down, it leaks sap which immediately makes the grass at the foot of the trunk whither and die. When it actually falls, a choking purple cloud is released, forcing you to raise your cloaks in protection. Although the Vermlings are still unhappy, they are significantly quieter, now they have seen what the tree did in death.

		You have long outstayed your welcome, and hastily leave the Vermling Gardens—you have no wish to cause further trouble. Ten minutes down the road, you find a Vermling peddler who attempts to sell you  one of the fruit from the Tree of Death. You hesitate for a second, torn between warning him and forcibly taking and destroying the fruit. Then, you sigh and walk away—one good deed is more than enough for the day.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<VermlingScout>(),
		ModelDB.Monster<VermlingShaman>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainReputationReward(2),
		new GainXPReward(15)
	];

	private IEnumerable<Obstacle> _bushesOfHope;
	private IEnumerable<Obstacle> _bushesOfForgiveness;
	private Objective _treeOfDeath;
	private Door _door1;
	private Door _door2;

	private CustomScenarioGoal _goal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal(countObjectives: false));
		_goal = await AddGoal(new CustomScenarioGoal(textParameters => "Destroy the Tree of Death.",
			hasProgress: true, maxProgress: 1));

		AddScenarioRule(textParameters =>
			$"The bushes marked {Icons.InlineMarker(Marker.Type.a, textParameters)} are the Bushes of Hope and cannot be destroyed. Whenever a figure adjacent to a Bush of Hope draws a negative modifier, treat it as a {Icons.Inline(Icons.GetAMDValue("+0"), textParameters)} instead.");

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();
		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		_bushesOfHope = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<Obstacle>());
		_bushesOfForgiveness = GameController.Instance.Map.GetMarkers(Marker.Type.b).Select(marker => marker.GetHexObject<Obstacle>());
		_treeOfDeath = GameController.Instance.Map.GetMarker(Marker.Type.c).GetHexObject<Objective>();
		_treeOfDeath.Init(GameController.Instance.SavedCampaign.Characters.Count * (GameController.Instance.SavedScenario.ScenarioLevel + 5),
			"Tree of Death");

		ScenarioEvents.AMDCardDrawnEvent.Subscribe(this, _bushesOfHope,
			parameters =>
				parameters.Type == AMDCardType.Value &&
				parameters.Value < 0 &&
				RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfHope.Select(bush => bush.Hex)).Any(),
			async parameters =>
			{
				parameters.SetValue(0);
				await GDTask.CompletedTask;
			}
		);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure == _treeOfDeath,
			async parameters =>
			{
				await _goal.AdjustProgress(1);
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.OpenedDoor == _door1)
		{
			AddScenarioRule(textParameters =>
				$"The bushes marked {Icons.InlineMarker(Marker.Type.b, textParameters)} are the Bushes of Forgiveness and cannot be destroyed. All figures adjacent to a Bush of Forgiveness add +1{Icons.Inline(Icons.Heal, textParameters)} to all heal abilities. Whenever a figure adjacent to a Bush of Forgiveness draws a {Icons.Inline(Icons.GetAMDValue("2x"), textParameters)} modifier, treat it as a {Icons.Inline(Icons.GetAMDValue("+0"), textParameters)} instead.");

			ScenarioEvents.AMDCardDrawnEvent.Subscribe(this, _bushesOfForgiveness,
				parameters =>
					parameters.Type == AMDCardType.Crit &&
					RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfForgiveness.Select(bush => bush.Hex)).Any(),
				async parameters =>
				{
					parameters.SetValue(0);
					await GDTask.CompletedTask;
				}
			);

			ScenarioEvents.AbilityStartedEvent.Subscribe(this, _bushesOfForgiveness,
				parameters =>
					parameters.AbilityState is HealAbility.State &&
					RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1).Intersect(_bushesOfForgiveness.Select(bush => bush.Hex)).Any(),
				async parameters =>
				{
					((HealAbility.State)parameters.AbilityState).AbilityAdjustHealValue(1);
					await GDTask.CompletedTask;
				}
			);

			await ShowText(
				"""
				You force your way through to the next section of the Vermling Gardens, avoiding killing the Vermlings where possible, but inevitably some get in the way.

				This part of the Gardens is long and thin, with different bushes weighed down with succulent fruit. The whole garden is beautifully tended and, on a different day, you imagine it would be very peaceful. Currently however, amongst the beauty, there are beasts—including one particularly nasty looking bear.

				A floral archway at the other end of the garden looks like it leads towards the tree—but you have to get there first.
				""");
		}
		else if(parameters.OpenedDoor == _door2)
		{
			await ShowText(
				"""
				You pass through the archway to see a great tree before you, by now heavily guarded. You can see the attraction of sampling its fruit—large, succulent and deep purple stone fruits hang from every branch. You attempt to engage with one of the shamans near the tree, warning of the danger, but you get nowhere.

				“Our writings talk of this tree. It bears fruit only once in a hundred years, and eating the fruit provides great insight and knowledge of the next world. We will defend it to the death!”

				You try to explain that they will become intimately familiar with the next world, whether they eat the fruit or fight you, but they will not be convinced. Never has a good deed seemed so difficult...
				""");
		}
	}
}