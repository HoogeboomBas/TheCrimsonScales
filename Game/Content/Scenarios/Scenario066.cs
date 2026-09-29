using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario066 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario066.tscn";

	public override int ScenarioNumber => 66;
	public override string Name => "Otherworldly Strangers";
	public override ClassModel ClassModel => ModelDB.Class<StarslingerModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<StarslingerModel>())];

	public override string IntroductionText =>
		"""
		Any self-respecting Aesther can move between realms at will; traversing the void is like a human learning to walk. However, just like humans, every now and then, you find yourself flat on your back for no reason.

		Today is one of those days. You don’t really know what happened, except that you were returning to Gloomhaven from your travels and there was... a shift, a slip, a glitch? Whatever it was, you pick yourself up next to the West barracks to find the warning bell tolling, the City Guard everywhere—and some demons that definitely shouldn’t be there.

		“Defense formation!” the sergeant bellows, before adding under his breath “What in the world have you brought here?!”

		You turn to fight the demons, with the guards supporting you. Then you notice strange shimmering lights and a crackle of energy, from around the square. You have a bad feeling this is about to get worse...
		""";

	public override string ConclusionText =>
		"""
		The City Guard tend to their wounded and you, slightly awkwardly, try to leave without attracting too much attention. You don’t escape the sergeant’s attention though.

		“Hey!” he shouts, and then, surprisingly, “Thank you! Your efforts stopped this becoming a much bigger problem.”

		You tentatively nod an acknowledgment and continue on your way when he shouts again. “One more thing. Next time you want to do your fancy teleporting thing—just walk instead!”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CityArcher>(),
		ModelDB.Monster<CityGuard>(),
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<HarrowerInfester>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<SunDemon>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<MoonleatherBoots>())
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		KillAllEnemiesScenarioGoal goal = await AddGoal(new KillAllEnemiesScenarioGoal(enemiesToBeSpawned: true));

		ScenarioRule cityWatchRule = AddScenarioRule("When 3 City Guards or Archers die, the scenario is lost.");

		int cityWatchLeftCount = 3;
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monster && monster.MonsterModel is CityArcher or CityGuard,
			async _ =>
			{
				cityWatchLeftCount--;
				cityWatchRule.SetText(_ => $"When {cityWatchLeftCount} City Guards or Archers die, the scenario is lost");

				if(cityWatchLeftCount <= 0)
				{
					await AbilityCmd.Lose();
				}
			}
		);

		AddScenarioRule(
			"All City Guards and City Archers are considered part of the City Watch and are allies to you and to each other and enemies to all other monsters.");

		ScenarioRule round3Rule = AddScenarioRule("Something will happen at the end of third and fifth rounds.");

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber == 3,
			async _ =>
			{
				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
				round3Rule.Remove();

				await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Normal,
					GameController.Instance.Map.GetMarkers(Marker.Type.e).First().Hex);
				await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Normal,
					GameController.Instance.Map.GetMarkers(Marker.Type.e).Last().Hex);

				ScenarioRule round5Rule = AddScenarioRule("Something will happen at the end of fifth round.");

				ScenarioEvents.RoundEndedEvent.Subscribe(this,
					parameters => parameters.RoundNumber == 5,
					async _ =>
					{
						round5Rule.Remove();
						ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

						await SpawnMonster(null, ModelDB.Monster<HarrowerInfester>(), MonsterType.Normal,
							GameController.Instance.Map.GetMarkers(Marker.Type.f).First().Hex);
						await SpawnMonster(null, ModelDB.Monster<HarrowerInfester>(), MonsterType.Normal,
							GameController.Instance.Map.GetMarkers(Marker.Type.f).Last().Hex);

						ScenarioRule somethingWillHappenRule = AddScenarioRule("Something will happen when all revealed enemies are dead.");

						ScenarioEvents.FigureKilledEvent.Subscribe(this,
							_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
							async _ =>
							{
								ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
								somethingWillHappenRule.Remove();

								await ShowText(
									"“Close those portals!” the sergeant screams at you—as quickly as one demon falls another is emerging to take its place. You decide not to tell him that you can’t fight and perform the necessary incantations at the same time, but instead pray for a break so that you can collapse the rifts. You nearly succeed, but just as they are shrinking in on themselves another wave of demons burst through, before the rifts vanish.");

								await SpawnMonster(null, ModelDB.Monster<SunDemon>(), MonsterType.Elite,
									GameController.Instance.Map.GetMarker(Marker.Type.a).Hex);
								await SpawnMonster(null, ModelDB.Monster<SunDemon>(), MonsterType.Elite,
									GameController.Instance.Map.GetMarker(Marker.Type.c).Hex);

								await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Elite,
									GameController.Instance.Map.GetMarker(Marker.Type.b).Hex);
								await SpawnMonster(null, ModelDB.Monster<EarthDemon>(), MonsterType.Elite,
									GameController.Instance.Map.GetMarker(Marker.Type.d).Hex);

								await goal.DisableEnemiesToBeSpawned();
							});
					});
			});
	}
}