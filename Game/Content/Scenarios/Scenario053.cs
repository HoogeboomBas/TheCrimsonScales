using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario053 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario053.tscn";

	public override int ScenarioNumber => 53;
	public override string Name => "Cave of Currents";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SideScenarioChain>();

	public override string PreviewText =>
	"""
		Kill all enemies.
	""";

	public override string IntroductionText =>
		"""
		As you rest near a river not far off the road, you notice that the normally calm river has a far stronger current than normal. Fallen trees and even boulders of various sizes are being ripped from the bank and dragged with the current into a small cave entrance not far away. Although your instincts are telling you this is not a good idea, you decide to investigate what’s causing this.

		As you enter the cave the ground unexpectedly starts to tremble and shift under your feet, and you slip and stumble down further into the cave until finally, you come to a stop in a small cavern.

		Above you, there is a gap in the ceiling, which is letting in rays of sunbeams coming from a gap in the ceiling. It would be beautiful; however, the shafts of light also reveal that you are not alone...
		""";

	public override string ConclusionText =>
		"""
		You watch as the watery shell soaks up all the water in the room before collapsing in on itself, soaking up the creature you defeated and leaving behind a small floating watery orb that seems to always have the water surrounding it in motion. With the last drop of water gone you notice some leftovers from the monsters you have slain.

		You grab anything of value, as well as the strange orb and start to climb the fallen down rocks to the surface. As you dry off, you resolve to be a bit less nosy about strange water currents in the future.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<SpittingDrake>(),
		ModelDB.Monster<WaterSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<SphereOfCurrents>()),
		new GainGoldEachReward(5)
	];

	private ScenarioRule _somethingWillHappenRule;

	private Door _door1;
	private Door _door2;

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await base.StartOfScenarioEffects(character);

		await AbilityCmd.AddCondition(null, character, Conditions.Muddle);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());

		_somethingWillHappenRule = AddScenarioRule("Something will happen when all revealed enemies are dead.");

		_door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).GetHexObject<Door>();
		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
			{
				foreach(Figure figure in GameController.Instance.Map.Figures)
				{
					if(figure.Alignment == Alignment.Monsters)
					{
						return false;
					}
				}

				return true;
			},
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

				await _door1.Unlock();

				//_doorsLockedRule.Remove();
				_somethingWillHappenRule.Remove();

				foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
				{
					if(hex.MapTile != GameController.Instance.Map.Rooms[0].MapTiles[0])
					{
						continue;
					}

					foreach(Water water in hex.GetHexObjectsOfType<Water>())
					{
						await water.Destroy();
					}

					foreach(Coin coin in hex.GetHexObjectsOfType<Coin>())
					{
						await coin.Destroy();
					}
				}

				await ShowText(
					"""
					As the last monster drops to the ground you hear a loud screeching noise from far underneath you. The floor trembles again and the water starts spouting upwards, reaching the ceiling in a huge fountain, before falling back down.

					As the water reaches the ground, you watch as the corpses of the monsters you have just slain get sucked into the water, completely disappearing within it before the water drains away moments later. With the final drop of water gone, you can see what looks like a pathway that you didn’t notice before.
					""");
			}
		);

		ScenarioEvents.DoorOpenedEvent.Subscribe(this,
			parameters => parameters.OpenedDoor == _door2,
			async parameters =>
			{
				await GameController.Instance.Map.Rooms[2].Reveal(parameters.OpenedDoor, parameters.PotentialOpener, false);

				List<Hex> hexes = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.Hex).ToList();

				ScenarioRule tempRule = AddScenarioRule(textParameters =>
					$"Each character is immediately teleported to the N1a map tile and gains {Icons.InlineCondition(Conditions.Muddle, textParameters)}.");

				foreach(Character character in GameController.Instance.CharacterManager.Characters)
				{
					Hex hex = await AbilityCmd.SelectHex(character,
						list =>
						{
							list.AddRange(hexes);
						},
						mandatory: true, hintText: $"Select a hex for {character.SavedCharacter.GetNameAndIcon(50)} to teleport to");

					hexes.Remove(hex);

					await AbilityCmd.Teleport(null, character, hex, true);

					await AbilityCmd.AddCondition(null, character, Conditions.Muddle);
				}

				tempRule.Remove();

// 				AddScenarioRule(textParameters =>
// 					$"""
// 					 At the start of every round, place 2 water tiles closest to hex {Icons.InlineMarker(Marker.Type.b, textParameters)}, all allied figures that occupy a water tile suffer {Icons.Inline(Icons.Damage)}1 for each water tile that could not be placed this way. {Icons.InlineCondition(Conditions.Muddle, textParameters)} and {Icons.Inline(Icons.Pull, textParameters, ignoreParametersColor: true)} all allied figures that occupy a water tile 1 hex towards hex {Icons.InlineMarker(Marker.Type.b, textParameters)}.
// 					 """);

				AddScenarioRule(textParameters =>
					$"""
					 At the start of every round, place 2 water tiles closest to hex {Icons.InlineMarker(Marker.Type.b, textParameters)}, all allied figures that occupy a water tile suffer {Icons.Inline(Icons.Damage)}1 for each water tile that could not be placed this way.
					 """);

				AddScenarioRule(textParameters =>
					$"""
					 At the start of every round, all allied figures that occupy a water tile gain {Icons.InlineCondition(Conditions.Muddle, textParameters)}.
					 """);

				ScenarioEvents.RoundStartBeforeCardSelectionEvent.Subscribe(this,
					roundStartParameters => true,
					async roundStartParameters =>
					{
						List<Hex> closestHexes = new List<Hex>();

						for(int i = 0; i < 2; i++)
						{
							Hex chosenHex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.FirstAlive(),
								list =>
								{
									List<Hex> possibleHexes = GameController.Instance.Map.Rooms[2].Hexes.Where(hex => hex.IsFeatureless()).ToList();
									Hex bHex = GameController.Instance.Map.GetMarker(Marker.Type.b).Hex;

									//int? minDistance = null;

									//hexes.Shuffle(GameController.Instance.VisualRNG);
									possibleHexes.Sort((otherHexA, otherHexB) =>
										RangeHelper.Distance(bHex, otherHexA).CompareTo(RangeHelper.Distance(bHex, otherHexB)));
									//Hex firstHex = possibleHexes.FirstOrDefault(hex => hex.IsEmpty() || (canHaveFeatures && hex.IsUnoccupied()));
									Hex firstHex = possibleHexes.FirstOrDefault(); //hex => hex.IsEmpty() || (canHaveFeatures && hex.IsUnoccupied()));

									if(firstHex == null)
									{
										return;
									}

									int distance = RangeHelper.Distance(bHex, firstHex);

									// if(minDistance != null && distance > minDistance)
									// {
									// 	continue;
									// }

									// if(minDistance == null || distance < minDistance)
									// {
									// 	list.Clear();
									// 	minDistance = distance;
									// }

									list.AddRange(possibleHexes.Where(hex => RangeHelper.Distance(bHex, hex) == distance));
								},
								true,
								$"Select a hex to create a water tile"
							);

							if(chosenHex != null)
							{
								await AbilityCmd.CreateDifficultTerrain(chosenHex,
									SceneLoader.LoadPackedScene("res://Content/OverlayTiles/DifficultTerrain/Water1H.tscn"));
							}
							else
							{
								// Each figure on a water tile suffers 1 damage if a water tile couldn't be placed
								foreach(Figure figure in GameController.Instance.Map.Figures)
								{
									if(figure.Alignment == Alignment.Characters && figure.Hex.HasHexObjectOfType<Water>())
									{
										await AbilityCmd.SufferDamage(figure, 1, null);
									}
								}
							}
						}
					}
				);

				await ShowText(
					"""
					As you go through the passageway opening, you hear a loud screeching noise, and the ground starts shaking more violently than before, causing a large rockfall that starts tumbling down towards you. You have no choice other than to rush through the pathway.

					You find yourself in a pitch dark place, the rocks blocking your only way out. Just as you try and get your bearings, the ceiling above you cracks open, pouring down water from above. The water builds up quickly and you are swept away by a strong current. After a short while the current picks up even more and you are swept into a whirlpool, swirling round and round.
					""");

				await ShowText(
					"""
					The ground trembles once again, this time, with such force that the ceiling starts to break down. Huge rocks fall down, again leaving holes in the ceiling that bring sudden, blinding light into the room. As your eyes adjust to the brightness, you notice a large creature standing right beside you, on the spot that you have been circling all this time. The large creature is surrounded by a swirling bowl-shaped shell of water that is in continuous motion.
					""");

				await ShowText(
					"""
					Suddenly you feel the remains of a Lurker floating past you which the creature in the middle picks out of the water and absorbs. As it does, you see the monster suddenly change form to a Lurker for a moment, before changing back to its original form, all the while remaining covered by water. The creature starts to hover higher and higher and with a screeching sound the water you are being dragged in starts to change current, suddenly spinning the other way. Your party is split and thrown to separate corners of the room. As you recover your footing, the water becomes still, and you turn to face this strange, threatening creature.
					""");
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		if(roomRevealedParameters.OpenedDoor == _door1)
		{
			_somethingWillHappenRule = AddScenarioRule("Something will happen when all revealed enemies are dead.");

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure.Alignment == Alignment.Monsters)
						{
							return false;
						}
					}

					return true;
				},
				async parameters =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

					await _door2.Unlock();

					//_doorsLockedRule.Remove();
					_somethingWillHappenRule.Remove();

					await ShowText(
						"""
						You are again met with a loud screeching noise that is a lot closer than last time. Already knowing what happened earlier, you prepare for the enormous water spout.
						""");
				}
			);

			await ShowText(
				"""
				The path goes deep underground, gradually revealing itself from the shafts of light above you. You shrug off what happened moments ago and get ready for battle, just as the light catches a glimpse of what seems like more trouble.
				""");
		}
	}
}