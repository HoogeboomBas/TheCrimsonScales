using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario043 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario043.tscn";
	public override int ScenarioNumber => 43;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	protected override ScenarioGoals CreateScenarioGoals() => new KillAllEnemiesScenarioGoals();

	private Door _door1;
	private Door _door2;
	private Door _door3;
	private Door _door4;
	private PressurePlate _pressurePlateA;
	private PressurePlate _pressurePlateB;
	private IEnumerable<PressurePlate> _pressurePlatesC;
	private IEnumerable<PressurePlate> _pressurePlatesD;
	private IEnumerable<PressurePlate> _pressurePlates;
	private readonly string _baseText = """
	                                    The obstacles in this scenario cannot be destroyed. Any character may spend one movement point while adjacent to a boulder to push the boulder one hex. Boulders may be pushed into adjacent unoccupied hexes or one unoccupied hex away further from character performing the push. If a boulder would be pushed into a trap or money token, it is crushed and remove the trap or money token from the board.

	                                    All doors start locked.

	                                    """;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<AshsteelGauntlets>());

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
		_door2 = marker2.GetHexObject<Door>();

		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
		_door3 = marker3.GetHexObject<Door>();

		Marker marker4 = GameController.Instance.Map.GetMarker(Marker.Type._4);
		_door4 = marker4.GetHexObject<Door>();

		UpdateScenarioText();

		_pressurePlateA = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<PressurePlate>();
		_pressurePlateB = GameController.Instance.Map.GetMarker(Marker.Type.b).GetHexObject<PressurePlate>();
		_pressurePlatesC = GameController.Instance.Map.GetMarkers(Marker.Type.c).Select(marker => marker.GetHexObject<PressurePlate>());
		_pressurePlatesD = GameController.Instance.Map.GetMarkers(Marker.Type.d).Select(marker => marker.GetHexObject<PressurePlate>());
		_pressurePlates =
			new[] { _pressurePlateA, _pressurePlateB }
				.Concat(_pressurePlatesC)
				.Concat(_pressurePlatesD);

		ScenarioEvents.OverlayTileMovedEvent.Subscribe(this,
			parameters => OverlayTileCanApplyParameters(parameters.OverlayTile),
			async parameters =>
			{
				await OverlayTileApplyParameters(parameters.OverlayTile);
			});

		ScenarioEvents.OverlayTileCreatedEvent.Subscribe(this,
			parameters => OverlayTileCanApplyParameters(parameters.OverlayTile),
			async parameters =>
			{
				await OverlayTileApplyParameters(parameters.OverlayTile);
			});

		ScenarioEvents.DuringMovementEvent.Subscribe(this,
			canApplyParameters => canApplyParameters.Performer is Character && canApplyParameters.AbilityState.MoveValue > 0 &&
			                      RangeHelper.GetHexesInRange(canApplyParameters.Performer.Hex, 1)
				                      .Any(hex => hex.HasHexObjectOfType<Boulder1HObstacle>()),
			async applyParameters =>
			{
				applyParameters.AbilityState.AdjustMoveValue(-1);

				Hex movedToHex = await AbilityCmd.RelocateOverlayTile(applyParameters.AbilityState,
					list => list.AddRange(RangeHelper.GetHexesInRange(applyParameters.Performer.Hex, 1)), (boulder, list) =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(boulder.Hex, 1)
							.Where(hex => hex.IsEmpty() || (hex.IsUnoccupied() && hex.GetHexObjectOfType<Trap>() != null)));
					}, [typeof(Boulder1HObstacle)], "Select a boulder to move");

				movedToHex.GetHexObjectOfType<Trap>()?.Destroy();
				foreach(Coin coin in movedToHex.GetHexObjectsOfType<Coin>())
				{
					await coin.Destroy();
				}
			},
			EffectType.Selectable,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Boulder 1h.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Move one adjacent boulder"));
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);
		UpdateScenarioText();
	}

	private void UpdateScenarioText()
	{
		string text = _baseText;
		if(_door1.Locked)
		{
			text +=
				$"When a boulder occupies pressure plate {Icons.Inline(Icons.GetMarker(Marker.Type.a))}, unlock door {Icons.Inline(Icons.GetMarker(Marker.Type._1))}. \n\n";
		}

		if(_door2.Locked)
		{
			text +=
				$"When a boulder occupies pressure plate {Icons.Inline(Icons.GetMarker(Marker.Type.b))}, unlock door {Icons.Inline(Icons.GetMarker(Marker.Type._2))}. \n\n";
		}

		if(_door3.Locked && (!_door1.Locked || !_door4.Locked))
		{
			text +=
				$"When a boulder occupies pressure plate {Icons.Inline(Icons.GetMarker(Marker.Type.c))}, unlock door {Icons.Inline(Icons.GetMarker(Marker.Type._3))}. \n\n";
		}

		if(_door4.Locked && (!_door2.Locked || !_door3.Locked))
		{
			text +=
				$"When a boulder occupies pressure plate {Icons.Inline(Icons.GetMarker(Marker.Type.d))}, unlock door {Icons.Inline(Icons.GetMarker(Marker.Type._4))}. \n\n";
		}

		text = text.TrimEnd('\n');
		base.UpdateScenarioText(text);
	}

	private bool OverlayTileCanApplyParameters(OverlayTile overlayTile)
	{
		return overlayTile is Boulder1HObstacle &&
		       _pressurePlates.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex);
	}

	private async GDTask OverlayTileApplyParameters(OverlayTile overlayTile)
	{
		if(_door1.Locked && _pressurePlateA.Hex == overlayTile.Hex)
		{
			await _door1.Unlock();
		}
		else if(_door2.Locked && _pressurePlateB.Hex == overlayTile.Hex)
		{
			await _door2.Unlock();
		}
		else if(_door3.Locked && _pressurePlatesC.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex))
		{
			await _door3.Unlock();
		}
		else if(_door4.Locked && _pressurePlatesD.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex))
		{
			await _door4.Unlock();
		}

		UpdateScenarioText();
	}
}