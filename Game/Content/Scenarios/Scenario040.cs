using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario040 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario040.tscn";

	public override int ScenarioNumber => 40;
	public override string Name => "Smugglers’ Hideout";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Having loosened a few tongues with a few drinks in the Sleeping Lion, you set out to find the Drake Porter and claim the ransom for him.

		A few hours from Gloomhaven, you find the entrance to a cave in the foothills of the Copperneck Mountains. You were expecting to have to search hard to find it, but you quickly notice cart tracks leading to it—swiftly followed by the smell of drake blood and rotting flesh.

		You cautiously enter the cave, but clearly not cautiously enough. Someone has seen you coming and released several of the drakes from their cage before, presumably, escaping through a sturdy door at the rear of the room. It seems slightly unfair to come here and kill them anyway, but it’s them or you.
		""";

	public override string ConclusionText =>
		"""
		Amongst the beating of wings, and the clawing and scratching of the drakes, you manage to pin down the smuggler and kill him. Before you leave, you make sure that the drakes are all dead too, figuring that Gloomhaven has enough monsters already. On the way out of the makeshift tannery, you help yourself to some of the drake hides—there’s no point leaving them to rot.

		Outside, you find a horse and cart and ride it back to Gloomhaven with the Porter’s body in the back (and the hides underneath) to claim the reward issued for his head, and quietly sell the hides. You could get used to this bounty hunting.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<DrakePorter>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SpittingDrake>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveGoldReward(50)
	];

	private Door _door1;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<DrakePorter>()));

		ScenarioRule doorRule = AddScenarioRule("The door is locked until all revealed enemies are killed.");

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<SteelHelmet>());

		ScenarioEvents.FigureKilledEvent.Subscribe(this, _door1,
			parameters => GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Monsters),
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this, _door1);

				doorRule.Remove();

				await _door1.Unlock();
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		Figure drakePorter =
			GameController.Instance.Map.Figures.First(figure => figure is Monster monster && monster.MonsterModel is DrakePorter);

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Monster monster &&
				monster.MonsterModel is RendingDrake or SpittingDrake,
			async parameters =>
			{
				if(!drakePorter.IsDead)
				{
					await AbilityCmd.SufferDamage(null, drakePorter, 2);
				}
			}
		);

		ScenarioRule drakePorterRule = AddScenarioRule("Every time you kill a drake, the Drake Porter suffers 2 damage.");

		ScenarioEvents.FigureKilledEvent.Subscribe(this, drakePorter,
			parameters =>
				parameters.Figure == drakePorter,
			async parameters =>
			{
				drakePorterRule.Remove();

				await GDTask.CompletedTask;
			}
		);

		await ShowText(
			"""
			Having dispatched the drakes, you force the door to see the Drake Porter standing in a fully set up tannery, with one wall completely given over to whole hides drying and curing. He is standing over yet more of the caged beasts, which again he lets free, causing the furious creatures to swarm towards you attacking all members of the party.

			The smuggler again retreats looking for a way out, but it doesn’t look like there is one. Getting him cornered should be easy—if you can get rid of these drakes.
			""");
	}
}