using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario002 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario002.tscn";
	public override int ScenarioNumber => 2;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario003>(true)];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private PressurePlate _pressurePlateA;
	private PressurePlate _pressurePlateB;
	private PressurePlate _pressurePlateC;
	private Door _door1;
	private Door _door2;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

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
						if(figure.Alignment == Alignment.Enemies)
						{
							await AbilityCmd.AddCondition(null, figure, Conditions.Strengthen);
						}
					}
				}

				await _door1.Unlock();

				await _pressurePlateA.Destroy();

				ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(this, _pressurePlateA);
			});

		UpdateScenarioText(
			$"The door is locked. When a character ends their turn on the pressure plate marked {Icons.Marker(Marker.Type.a)}, " +
			$"all enemies occupying the I2A map tile gain {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))} and the door is permanently unlocked.");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.OpenedDoor == _door1)
		{
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
				});

			UpdateScenarioText(
				$"The door is locked. When a character ends their turn on the pressure plate marked {Icons.Marker(Marker.Type.b)} the door is permanently unlocked " +
				$"and all figures occupying the H1A map tile perform a “{Icons.Inline(Icons.Heal)} 2, Self” ability.");
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
				});

			UpdateScenarioText(
				$"The pressure plate marked {Icons.Marker(Marker.Type.c)} activates the Electric Current. " +
				"When a character ends their turn on this pressure plate, all figures occupying a water hex in the E1B tile immediately have their current hit points reduced to 1.");
		}
	}
}