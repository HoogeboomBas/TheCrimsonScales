using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario043 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario043.tscn";

	public override int ScenarioNumber => 43;
	public override string Name => "Mansion Maze";

	public override List<ScenarioLink> Links => [GloomhavenLink.Instance];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Councilman Raksani’s lush lawns stretch away to one side, and you can see his enormous mansion to the other. In front of you lies the labyrinth he so proudly showed off to you. The merchant is a very wealthy and influential man, so you humor him—but a garden maze? This is a child’s game.

		You enter the labyrinth, but instead of a simple hedge maze, walls and boulders block the way. Then you see the artillery…
		""";

	public override string ConclusionText =>
		"""
		You disable the last enemy and escape. You had planned to appear shocked and relieved to escape to Councilman Raksani regardless but the truth is, you are. He looks at you with a twinkle in your eye, and you realize that he has not built up his riches without being a sound judge of character.

		“A little trickier than you may have thought, perhaps?!” he says with a wink. “I heard you were good, and I thought it was worth finding out for myself. I hope this trinket is sufficient for your troubles?” Feeling a little like you were tricked and put in harm’s way for someone’s enjoyment; and feeling a lot like you just got paid for exercising your natural talents, you accept graciously and head back to town. Just another day in Gloomhaven.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtillery>(),
		ModelDB.Monster<CityGuard>(),
		ModelDB.Monster<StoneGolem>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainProsperityReward(1),
		new GainRandomOrbEachReward()
	];

	private Door _door1;
	private Door _door2;
	private Door _door3;
	private Door _door4;
	private PressurePlate _pressurePlateA;
	private PressurePlate _pressurePlateB;
	private IEnumerable<PressurePlate> _pressurePlatesC;
	private IEnumerable<PressurePlate> _pressurePlatesD;
	private IEnumerable<PressurePlate> _pressurePlates;

	private ScenarioRule _door1Rule;
	private ScenarioRule _door2Rule;
	private ScenarioRule _door3Rule;
	private ScenarioRule _door4Rule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		AddScenarioRule(
			"""
			The obstacles in this scenario cannot be destroyed. Any character may spend one movement point while adjacent to a boulder to move the boulder one hex in any direction. Boulders may be pushed into adjacent unoccupied hexes or one unoccupied hex away further from character performing the push. If a boulder would be pushed into a trap or money token, it is crushed and the trap or money token is removed.
			"""
		);

		_door1Rule = AddScenarioRule(textParameters =>
			$"When a boulder occupies the pressure plate marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, unlock door {Icons.InlineMarker(Marker.Type._1, textParameters)}.");
		_door2Rule = AddScenarioRule(textParameters =>
			$"When a boulder occupies the pressure plate marked {Icons.InlineMarker(Marker.Type.b, textParameters)}, unlock door {Icons.InlineMarker(Marker.Type._2, textParameters)}.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<AshsteelGauntlets>());

		Marker marker1 = GameController.Instance.Map.GetMarker(Marker.Type._1);
		_door1 = marker1.GetHexObject<Door>();

		Marker marker2 = GameController.Instance.Map.GetMarker(Marker.Type._2);
		_door2 = marker2.GetHexObject<Door>();

		Marker marker3 = GameController.Instance.Map.GetMarker(Marker.Type._3);
		_door3 = marker3.GetHexObject<Door>();

		Marker marker4 = GameController.Instance.Map.GetMarker(Marker.Type._4);
		_door4 = marker4.GetHexObject<Door>();

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
			canApplyParameters =>
				canApplyParameters.Performer is Character && canApplyParameters.AbilityState.MoveValue > 0 &&
				RangeHelper.GetHexesInRange(canApplyParameters.Performer.Hex, 1)
					.Any(hex => hex.HasHexObjectOfType<Boulder1HObstacle>()),
			async applyParameters =>
			{
				applyParameters.AbilityState.AdjustMoveValue(-1);

				Hex movedToHex = await AbilityCmd.RelocateOverlayTile(applyParameters.AbilityState,
					overlayTiles => overlayTiles.AddRange(RangeHelper.GetOverlayTilesInRange<Boulder1HObstacle>(applyParameters.Performer, 1)),
					(boulder, list) =>
					{
						list.AddRange(RangeHelper.GetHexesInRange(boulder.Hex, 1)
							.Where(hex => hex.IsEmpty() || (hex.IsUnoccupied() && hex.GetHexObjectOfType<Trap>() != null)));
					}, "Select a boulder to move");

				if(movedToHex != null)
				{
					foreach(Trap trap in movedToHex.GetHexObjectsOfType<Trap>())
					{
						await trap.Destroy();
					}
					foreach(Coin coin in movedToHex.GetHexObjectsOfType<Coin>())
					{
						await coin.Destroy();
					}
				}
			},
			EffectType.Selectable,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: new IconEffectButton.Parameters("res://Art/OverlayTiles/Boulder 1h.png"),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Move one adjacent boulder")
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(_door3Rule == null && _door3.Locked && (!_door1.Locked || !_door4.Locked))
		{
			_door3Rule = AddScenarioRule(textParameters =>
				$"When a boulder occupies the pressure plate marked {Icons.InlineMarker(Marker.Type.c, textParameters)}, unlock door {Icons.InlineMarker(Marker.Type._3, textParameters)}.");
		}

		if(_door4Rule == null && _door4.Locked && (!_door2.Locked || !_door3.Locked))
		{
			_door4Rule = AddScenarioRule(textParameters =>
				$"When a boulder occupies the pressure plate marked {Icons.InlineMarker(Marker.Type.d, textParameters)}, unlock door {Icons.InlineMarker(Marker.Type._4, textParameters)}.");
		}
	}

	private bool OverlayTileCanApplyParameters(OverlayTile overlayTile)
	{
		return
			overlayTile is Boulder1HObstacle &&
			_pressurePlates.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex);
	}

	private async GDTask OverlayTileApplyParameters(OverlayTile overlayTile)
	{
		if(_door1.Locked && _pressurePlateA.Hex == overlayTile.Hex)
		{
			await _door1.Unlock();

			_door1Rule.Remove();
		}
		else if(_door2.Locked && _pressurePlateB.Hex == overlayTile.Hex)
		{
			await _door2.Unlock();

			_door2Rule.Remove();
		}
		else if(_door3.Locked && _pressurePlatesC.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex))
		{
			await _door3.Unlock();

			_door3Rule.Remove();
		}
		else if(_door4.Locked && _pressurePlatesD.Any(pressurePlate => pressurePlate.Hex == overlayTile.Hex))
		{
			await _door4.Unlock();

			_door4Rule.Remove();
		}
	}
}