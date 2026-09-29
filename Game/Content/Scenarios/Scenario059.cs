using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class Scenario059 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario059.tscn";

	public override int ScenarioNumber => 59;
	public override string Name => "Forest Invaders";
	public override ClassModel ClassModel => ModelDB.Class<ChieftainModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<ChieftainModel>())];

	public override string IntroductionText =>
		"""
		You have always known you were special. The elders in your village, the ancient books detailing Orchid history, or just the fact that you have never seen anyone else with your talent for relating to, and controlling, the beasts of the forest. Certainly no-one outside the noble Orchids has the self-control and mastery to convince wild beasts to be ridden like domestic animals.

		Which is why it is strange and alarming to hear reports of not just one, but several animal riders in the forest near to your camp. Worse, these ‘other’ riders are indiscriminately causing havoc, and rumor has it that they are Vermlings, barely more than creatures of the forest themselves.

		As you patrol the area, you hear a crashing of branches and see a giant snake crashing wildly through the undergrowth with a Vermling clinging onto its back for dear life. These have no noble communication with the animals, as you do—they are savages—but this is a savage with weapons, and by the sound of it there are more approaching. You cannot communicate with the snake while it is trying to thrash the Vermling off, so you summon your own creatures to rid the forest of this barbarian and the animal it has somehow corrupted.
		""";

	public override string ConclusionText =>
		"""
		You slay the Shaman and, as he falls, two strange Armlets fall from his body. It’s unclear whether it is these that allowed him to exert some level of control over the beasts, but certainly once he is slain, no more beasts enter the clearing you are in, and all is quiet.

		Picking up the armlets, you ride off to spread the news, hopeful that once again, your power to control nature is unique in these parts.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<VermlingScout>(),
		ModelDB.Monster<VermlingShaman>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<NatureArmlets>())
	];

	private Hex _markerAHex;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		KillAllEnemiesScenarioGoal goal = await AddGoal(new KillAllEnemiesScenarioGoal(enemiesToBeSpawned: true));

		_markerAHex = GameController.Instance.Map.GetMarker(Marker.Type.a).Hex;

		await SpawnMonsterAndMount<GiantViper, VermlingScout>(_markerAHex);

		AddScenarioRule(
			"Mounting follows all of the Orchid Chieftain’s rules for Mount. While the figure is mounted, it will not attempt to move off of its mount. The mounting figure cannot be targeted or focused on with any abilities until its mount has died.");

		ScenarioRule scoutRule = AddScenarioRule("The Vermling Scouts are considered to be mounted on the Giant Vipers.");

		ScenarioRule spawnRule = AddScenarioRule(textParameters =>
			$"At the end of the second round, spawn one normal Vermling Scout and one elite Giant Viper at {Icons.InlineMarker(Marker.Type.a, textParameters)}.");

		ScenarioRule somethingWillHappenRule1 = AddScenarioRule("Something will happen when all enemies are dead after the second round.");

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber == 2,
			async _ =>
			{
				spawnRule.Remove();
				await SpawnMonsterAndMount<GiantViper, VermlingScout>(_markerAHex);

				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
					async _ =>
					{
						await goal.DisableEnemiesToBeSpawned();

						scoutRule.Remove();
						somethingWillHappenRule1.Remove();

						await ShowText(
							"You destroy the two Vermlings and their viper mounts when you hear howling in the distance. Another Vermling, this time riding a large wolf-like creature, comes careering through the undergrowth. He is either screaming a battle-cry, or just screaming in terror—it’s hard to tell.");

						ScenarioRule houndRule = AddScenarioRule("The Vermling Shaman is considered to be mounted on the Hound.");
						ScenarioRule somethingWillHappenRule2 = AddScenarioRule("Something will happen when all enemies are dead.");

						await SpawnMonsterAndMount<Hound, VermlingShaman>(GameController.Instance.Map.GetMarker(Marker.Type.b).Hex);

						ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

						ScenarioEvents.FigureKilledEvent.Subscribe(this,
							_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
							async _ =>
							{
								houndRule.Remove();
								somethingWillHappenRule2.Remove();

								await ShowText(
									"""
									You fight off the wolf-riding Vermling and barely catch your breath when you see another Vermling appear. This one appears to be the mastermind and leader of this group, judging by the dented crown askew on top of his head, and the shaman’s staff he is somehow carrying.

									He would strike a slightly comical figure, were it not for the fact that he was riding an enormous bear, and seems to have more control than the others.
									""");

								AddScenarioRule("The Vermling Shaman is considered to be mounted on the Cave Bear.");

								await SpawnMonsterAndMount<CaveBear, VermlingShaman>(GameController.Instance.Map.GetMarker(Marker.Type.c).Hex,
									MonsterType.Elite);

								ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
							});
					});
			});
	}

	private async GDTask SpawnMonsterAndMount<TMount, TMounting>(Hex spawnHex, MonsterType mountingMonsterType = MonsterType.Normal)
		where TMount : MonsterModel
		where TMounting : MonsterModel
	{
		Monster mount = await SpawnMonster(null, ModelDB.Monster<TMount>(), MonsterType.Elite, spawnHex);
		Monster mounting = await SpawnMonster(null, ModelDB.Monster<TMounting>(), mountingMonsterType,
			GameController.Instance.Map.Hexes.Select(hex => hex.Value).First(hex => hex.IsEmpty()));

		MonsterMountTrait trait = new MonsterMountTrait(mounting);
		await trait.Activate(mount);

		ScenarioEvents.FigureKilledEvent.Subscribe(this, mount,
			parameters => parameters.Figure == mount,
			async _ =>
			{
				await trait.Deactivate(mount);
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this, mount);
			});
	}


	private class MonsterMountTrait(Figure mountingFigure) : FigureTrait
	{
		private const string MountedAnchorName = "MountedAnchor";

		private bool _mounted;

		public override async GDTask Activate(Figure figure)
		{
			await base.Activate(figure);

			Node2D mountedAnchor = new Node2D();
			mountedAnchor.SetName(MountedAnchorName);
			mountedAnchor.SetPosition(new Vector2(70f, -70f));
			mountedAnchor.SetScale(0.7f * Vector2.One);
			figure.AddChild(mountedAnchor);

			// Allow stopping movement in the same hex to mount
			ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Subscribe(figure, this,
				parameters =>
					(parameters.PotentialAbilityState is MoveAbility.State &&
					 parameters.OtherFigure == figure &&
					 parameters.Figure == mountingFigure) ||
					(_mounted &&
					 parameters.Figure == figure &&
					 parameters.OtherFigure == mountingFigure),
				parameters =>
				{
					parameters.SetCanStopAt();
				}
			);

			// Follow the mount when it moves or being forcefully moved
			ScenarioEvents.MoveTogetherEvent.Subscribe(figure, this,
				parameters => parameters.Performer == figure && _mounted,
				async parameters =>
				{
					parameters.AddOtherFigure(mountingFigure);
					parameters.SetTriggerHexEffects(false);

					await GDTask.CompletedTask;
				}
			);

			// Mounting is done by the owner ending their movement in a hex occupied by the mount
			ScenarioEvents.AbilityPerformedEvent.Subscribe(figure, this,
				parameters =>
					!_mounted &&
					parameters.AbilityState is MoveAbility.State &&
					parameters.Performer == mountingFigure &&
					mountingFigure.Hex == figure.Hex &&
					!ScenarioCheckEvents.IsMountedCheckEvent.Fire(new ScenarioCheckEvents.IsMountedCheck.Parameters(mountingFigure)).IsMounted,
				async _ =>
				{
					await Mount(figure);
				}
			);

			ScenarioEvents.FigureExitingHexEvent.Subscribe(figure, this,
				parameters =>
					parameters.Figure == mountingFigure &&
					_mounted &&
					parameters.Hex == figure.Hex,
				async _ =>
				{
					await Dismount(figure);

					// Owner is exiting the hex the mount is on, so they are no longer mounted
					figure.UpdateInitiative();
				}
			);

			// Returning mounted status for other effects and abilities
			ScenarioCheckEvents.IsMountedCheckEvent.Subscribe(figure, this,
				parameters => parameters.Figure == mountingFigure,
				parameters =>
				{
					if(_mounted)
					{
						parameters.SetMount(figure);
					}
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(mountingFigure, this,
				parameters => parameters.PotentialTarget == mountingFigure && _mounted,
				parameters =>
				{
					parameters.SetCannotBeFocused();
				});

			ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(mountingFigure, this,
				parameters => parameters.PotentialTarget == mountingFigure && _mounted,
				parameters =>
				{
					parameters.SetCannotBeTargeted();
				});

			ScenarioEvents.AbilityStartedEvent.Subscribe(mountingFigure, this,
				parameters => _mounted && parameters.Performer == mountingFigure && parameters.AbilityState is MoveAbility.State,
				async parameters =>
				{
					parameters.SetIsBlocked(true);

					await GDTask.CompletedTask;
				});

			await Mount(figure);
		}

		public override async GDTask Deactivate(Figure figure)
		{
			await base.Deactivate(figure);

			if(_mounted)
			{
				await Dismount(figure);
			}

			ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Unsubscribe(figure, this);
			ScenarioEvents.MoveTogetherEvent.Unsubscribe(figure, this);
			ScenarioEvents.AbilityPerformedEvent.Unsubscribe(figure, this);
			ScenarioEvents.FigureExitingHexEvent.Unsubscribe(figure, this);
			ScenarioCheckEvents.IsMountedCheckEvent.Unsubscribe(figure, this);
			ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(mountingFigure, this);
			ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(mountingFigure, this);
			ScenarioEvents.AbilityStartedEvent.Unsubscribe(mountingFigure, this);
		}

		private async GDTask Dismount(Figure figure)
		{
			_mounted = false;

			mountingFigure.FigureViewComponent.SetCanAdjustViewPosition(true);
			mountingFigure.Reparent(GameController.Instance.Map);
			mountingFigure.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
			mountingFigure.TweenGlobalPosition(figure.Hex.GlobalPosition, 0.2f)
				.SetEasing(Easing.InBack).PlayFastForwardable();
			await GDTask.DelayFastForwardable(0.3f);
		}

		private async GDTask Mount(Figure figure)
		{
			if(figure.Hex != mountingFigure.Hex)
			{
				await AbilityCmd.EnterHex(null, mountingFigure, mountingFigure, figure.Hex, false, true);
			}

			_mounted = true;

			mountingFigure.FigureViewComponent.SetCanAdjustViewPosition(false);
			mountingFigure.Reparent(figure.GetNode<Node2D>(MountedAnchorName));
			mountingFigure.TweenScale(1f, 0.3f).SetEasing(Easing.InOutBack).PlayFastForwardable();
			mountingFigure.TweenPosition(Vector2.Zero, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
			await GDTask.DelayFastForwardable(0.3f);
		}
	}
}