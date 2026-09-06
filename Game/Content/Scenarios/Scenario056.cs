using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class Scenario056 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario056.tscn";

	public override int ScenarioNumber => 56;
	public override string Name => "Disarm the Machine";
	public override ClassModel ClassModel => ModelDB.Class<BombardModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<BombardModel>())];

	public override string IntroductionText =>
		"""
		Before yesterday the title of First Sergeant of the Bombard didn’t exist but today it’s something to be proud of. After recently being appointed to lead the artillery division of Gloomhaven’s defenses, you make your way along the Northern Wall to inspect the recent installations.

		You’re feeling fairly confident about your latest machination ARCS, otherwise known as Automated Rotational Cannon System. What’s the best way to be in more places at once, firing more cannons at once? That’s easy, build a clockwork device that can do it all. After recent Inox raids, the city is investing heavily in new technology. You’re hoping today’s display will show them Quatryls have a place on the battlefield.

		With the pull of one lever ARCS begins to creak, groan and eventually hum. Large chains pull the cannons along the outer wall. Small arms mechanically reach to the fuse, ignite and prepare for the combustion. It seems to be running smoothly with a few test shots hitting targets out in the distant field. There’s a crack and then a whoosh.

		Suddenly, the little alarm bell begins ringing just before steam bursts through the one exhaust valve. ARCS changes it’s trajectory aiming back at the city! If this causes any damage, your title of First Sergeant will be a very short-lived one.
		""";

	public override string ConclusionText =>
		"It’s heartbreaking to see months of labor turned into heaps of metal debris. Out of the plumes of smoke a cog rolls across the stones before falling with a clang. Its clear ARCS will need much more testing before it’s ready but at least Gloomhaven sustained no major damage.";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtilleryScenario056>(),
		ModelDB.Monster<StoneGolem>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<ChainCannon>())
	];

	private readonly List<Hex> _path = [];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		_path.AddRange(GameController.Instance.Map.GetMarkers(Marker.Type.b).Select(marker => marker.Hex));


		AddScenarioRule("The obstacles in this scenario cannot be moved or destroyed.");
		List<ScenarioRule> artilleryRules =
		[
			AddScenarioRule(
				"The Ancient Artillery are affixed to a rotating conveyor system. At the beginning of each round, each Ancient Artillery moves one hex clockwise, regardless of any negative conditions."),
			AddScenarioRule(textParameters =>
				$"All Ancient Artillery have a base {Icons.Inline(Icons.Range, textParameters)} value of 4.")
		];

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
			async _ =>
			{
				foreach(Marker marker in GameController.Instance.Map.GetMarkers(Marker.Type.a))
				{
					await SpawnMonster(null, ModelDB.Monster<StoneGolem>(), MonsterType.Normal, marker.Hex);
				}

				foreach(ScenarioRule scenarioRule in artilleryRules)
				{
					scenarioRule.Remove();
				}

				AddScenarioRule(textParameters =>
					$"The Stone Golems perform all melee attacks as if they were {Icons.Inline(Icons.Range, textParameters)}3 attacks.");
			});

		ScenarioEvents.DuringAttackEvent.Subscribe(this,
			parameters => parameters.Performer is Monster monster && monster.MonsterModel is StoneGolem &&
			              parameters.AbilityState.SingleTargetRangeType == RangeType.Melee,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetSetRangeType(RangeType.Range);
				parameters.AbilityState.SingleTargetAdjustRange(2);

				await GDTask.CompletedTask;
			});

		ScenarioEvents.RoundStartBeforeCardSelectionEvent.Subscribe(this,
			_ => true,
			async _ =>
			{
				foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure =>
					        figure is Monster monster && monster.MonsterModel is AncientArtillery))
				{
					int currentIndex = _path.IndexOf(figure.Hex);
					if(currentIndex < 0)
					{
						continue;
					}

					Hex hex = currentIndex == _path.Count - 1 ? _path[0] : _path[currentIndex + 1];


					await AbilityCmd.ExitHex(null, figure, null);

					Node2D moveParent = GameController.Instance.MoveParent;
					Node2D previousParent = figure.GetParent<Node2D>();
					moveParent.SetGlobalPosition(figure.Hex.GlobalPosition);
					figure.Reparent(moveParent);
					await moveParent.TweenGlobalPosition(hex.GlobalPosition, 0.2f).PlayFastForwardableAsync();

					figure.Reparent(previousParent);

					await moveParent.TweenGlobalPosition(hex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine).PlayFastForwardableAsync();

					await AbilityCmd.EnterHex(null, figure, null, hex, true, true);
				}
			});
	}
}