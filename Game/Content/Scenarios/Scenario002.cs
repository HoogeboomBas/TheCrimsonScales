using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario002 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";

	public override int ScenarioNumber => 2;
	public override string Name => "Underground Channels";

	public override List<ScenarioLink> Links => [new ScenarioLink<Scenario001>()];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario003>(true)];

	public override string IntroductionText =>
		"""
		You cautiously enter the channel. It was originally formed of bricks, like a sewer, but is now so overgrown that it appears organic. Also like a sewer, it is dark, damp and smells terrible.

		You light torches to illuminate your passage, and see that there is some form of ancient engineering, presumably to control the rate of whatever water used to flow through this channel and into the lake. You also notice that you are not alone, the dark and damp has attracted the usual underground creatures, plus some out of place Inox and some more of the Water Spirits.

		You see a barred door ahead, locked with some ancient Quatryl technology. Whatever is being guarded, it is through there.
		""";

	public override string ConclusionText =>
		"""
		With the channel cleared, you decide to retreat out of the chamber before the ancient Quatryl electronics give out altogether. You make it into the second chamber, when you hear a sudden rushing of water.

		You duck into a recess just as an enormous wave of water rushes through the main channel. It rises quickly, and you start to regret unlocking the doors. Soon, the water is rising up to your knees, then your waist. As you begin to worry about having to swim for it, the wall behind you collapses and the rush of water sweeps you into another cavern.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<WaterSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario003>())
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private PressurePlate _pressurePlateA;
	private PressurePlate _pressurePlateB;
	private PressurePlate _pressurePlateC;
	private Door _door1;
	private Door _door2;

	private ScenarioRule _scenarioRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		Marker markerA = GameController.Instance.Map.GetMarker(Marker.Type.a);
		_pressurePlateA = markerA.GetHexObject<PressurePlate>();

		Marker markerB = GameController.Instance.Map.GetMarker(Marker.Type.b);
		_pressurePlateB = markerB.GetHexObject<PressurePlate>();

		Marker markerC = GameController.Instance.Map.GetMarker(Marker.Type.c);
		_pressurePlateC = markerC.GetHexObject<PressurePlate>();

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
		_door2 = marker2.GetHexObject<Door>();

		ScenarioEvents.FigureTurnEndingEvent.Subscribe(this, _pressurePlateA,
			canApplyParameters => canApplyParameters.Figure is Character character && character.Hex == _pressurePlateA.Hex,
			async applyParameters =>
			{
				foreach(Hex hex in GameController.Instance.Map.Rooms[0].Hexes)
				{
					foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
					{
						if(figure.Alignment == Alignment.Monsters)
						{
							await AbilityCmd.AddCondition(null, figure, Conditions.Strengthen);
						}
					}
				}

				await _door1.Unlock();

				await _pressurePlateA.Destroy();

				ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(this, _pressurePlateA);

				_scenarioRule.Remove();
			}
		);

		_scenarioRule = AddScenarioRule(textParameters =>
			$"""
			 The door is locked. When a character ends their turn on the pressure plate marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, all enemies occupying the I2A map tile gain {Icons.InlineCondition(Conditions.Strengthen, textParameters)} and the door is permanently unlocked.
			 """);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.OpenedDoor == _door1)
		{
			await ShowText(
				"""
				Stepping onto the plate caused a grinding of ancient gears and as you approach the door, you can see that the thick iron bars that once locked the door have been retracted. With a mighty shove, you are able to heave open the metal gate to reveal another chamber, with yet more dark beings ready to protect their adopted lair.
				""");

			ScenarioEvents.FigureTurnEndingEvent.Subscribe(this, _pressurePlateB,
				canApplyParameters => canApplyParameters.Figure is Character character && character.Hex == _pressurePlateB.Hex,
				async applyParameters =>
				{
					foreach(Hex hex in GameController.Instance.Map.Rooms[1].Hexes)
					{
						foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
						{
							ActionState actionState =
								new ActionState(figure, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
							await actionState.Perform();
						}
					}

					await _door2.Unlock();

					await _pressurePlateB.Destroy();

					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(this, _pressurePlateB);

					_scenarioRule.Remove();
				}
			);

			_scenarioRule = AddScenarioRule(textParameters =>
				$"""
				 The door is locked. When a character ends their turn on the pressure plate marked {Icons.InlineMarker(Marker.Type.b, textParameters)} the door is permanently unlocked and all figures occupying the H1A map tile perform a “{Icons.Inline(Icons.Heal, textParameters)}2, self” ability.
				 """);
		}

		if(parameters.OpenedDoor == _door2)
		{
			ScenarioEvents.FigureTurnEndingEvent.Subscribe(this, _pressurePlateC,
				canApplyParameters => canApplyParameters.Figure is Character character && character.Hex == _pressurePlateC.Hex,
				async applyParameters =>
				{
					foreach(Hex hex in GameController.Instance.Map.Rooms[2].Hexes)
					{
						if(hex.HasHexObjectOfType<Water>())
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
							{
								figure.SetHealth(1);
							}
						}
					}

					await _pressurePlateC.Destroy();

					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(this, _pressurePlateC);

					_scenarioRule.Remove();
				}
			);

			_scenarioRule = AddScenarioRule(textParameters =>
				$"""
				 The pressure plate marked {Icons.InlineMarker(Marker.Type.c, textParameters)} activates the Electric Current. When a character ends their turn on this pressure plate, all figures occupying a water hex in the E1B tile immediately have their current hit points reduced to 1.
				 """);
		}
	}
}