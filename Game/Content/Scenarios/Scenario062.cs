using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario062 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario062.tscn";

	public override int ScenarioNumber => 62;
	public override string Name => "Hollow Redemption";
	public override ClassModel ClassModel => ModelDB.Class<HollowpactModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<HollowpactModel>())];

	public override string IntroductionText =>
		"""
		“The power was worth it”, you told yourself. You were lying to yourself back then, and the years have only proven that lie to you. Your indentured servitude to Void-born powers of corruption chafes at you.

		This lack of freedom is galling, but you know inescapably that your body will disintegrate if those void powers stop bolstering your defenses. Even in spite of these defenses, you can feel the Void energy eating away at your body and taking its toll on your mind as time passes onward.

		You’ve been searching for a solution—any chance to free yourself and live the remainder of your life on your own terms. Your best hope is a whispered tale of an ancient Savvas shrine deep in the Copperneck Mountains with potent mending powers.

		After more than a few ancient scrolls read, dozens of cryptic runes deciphered and a bevy of coins dropped in the right pockets, you’ve managed to find a promising lead on the shrine and procured a crudely-drawn map of the area.

		After days of hiking the Copperneck Mountains, you find a secluded plateau hidden by overgrowth. You spin a tendril of void energy into a blade and slice through the thick vines blocking the entranceway. At least all this power has its uses.

		Across the plateau, you see a strangely smooth and unnatural rock formation. You may have found the spring, but the large angry bear charging at you doesn’t appear interested in making your trip to the cave easy.
		""";

	public override string ConclusionText =>
		"""
		With the Aesther slain, you are free to complete your task. As you begin the ritual to cleanse your Voidheart, you feel immense pain, as if your insides were being eaten away. The pain is only made bearable by the powerful healing force of the spring fighting against the Void corruption that fills you to your hollowed core.

		Slowly, a transformation takes place inside the Voidheart. Instead of exuding an aura of corruption, it feels cleaner. It continues to glow with an otherworldly light, but it looks somehow subdued, less malevolent. You return the cleansed Voidheart to the hollows of your cragged chest and you immediately feel a mighty bulwark against the corrupt energies inside you.

		As long as this cleansed Voidheart’s power holds, you will no longer be beholden to the servants of the Void for your survival. The Void energy will still gradually eat away at your body and mind. Nothing can save you from that fate—not anymore—but at least you’ve earned a few years of relative freedom until that time comes.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AestherAshblade>(),
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<ForestImp>(),
		ModelDB.Monster<YourShadow>(),
		ModelDB.Monster<VermlingShaman>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<CleansedVoidheart>())
	];

	private CustomScenarioGoal _goal;
	private ScenarioRule _voidEnergyRule;
	private ScenarioRule _doorRule;
	private ScenarioRule _shamanDeadRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(_ => "Cleanse your Voidheart", maxProgress: 1));

		Hollowpact hollowpact = (Hollowpact)GameController.Instance.CharacterManager.Characters.First(character => character is Hollowpact);
		hollowpact.GainVoidEnergy();

		_voidEnergyRule = AddScenarioRule($"You begin the scenario with 1{Icons.Inline(Hollowpact.VoidEnergy)}.");
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);
		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[1])
		{
			_voidEnergyRule.Remove();

			_doorRule = AddScenarioRule(textParameters =>
				$"Door {Icons.InlineMarker(Marker.Type._1, textParameters)} is locked and cannot be opened until instructed.");

			_shamanDeadRule = AddScenarioRule("Something will happen when the Vermling Shaman is killed.");

			await ShowText(
				"""
				You cut your way through another mass of overgrown vines and find yourself face-to-face with a strangely-dressed Vermling shaman. This shaman must have charmed some of the local wildlife to keep guard over this area. Nonetheless, you’ve come too far to be scared away by this gaggle of inferior creatures.

				Your destination looms ahead of you, but you don’t yet see any way to enter it. The time to puzzle that out will come later, over this shaman’s dead body.
				""");

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters => parameters.Figure is Monster monster && monster.MonsterModel is VermlingShaman,
				async parameters =>
				{
					Door door1 = GameController.Instance.Map.GetMarker(Marker.Type._1).Hex.GetHexObjectOfType<Door>();
					await door1.Unlock();
					await door1.Open(parameters.PotentialKiller);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
				});
		}
		else if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
		{
			_doorRule.Remove();
			_shamanDeadRule.Remove();

			AddScenarioRule(textParameters =>
				$"""
				 This cavern contains a powerful healing spring. Perform {Icons.Inline(Icons.Heal, textParameters)}3, self at the end of each of your turns if you occupy one of these water tiles. Additionally, whenever you would gain {Icons.InlineCondition(Conditions.Muddle, textParameters)} while occupying one of these water tiles, gain {Icons.InlineCondition(Conditions.Strengthen, textParameters)} instead.
				 Similarly, whenever you would gain {Icons.InlineCondition(Conditions.Wound1, textParameters)} while occupying one of these water tiles, gain {Icons.InlineCondition(Conditions.Regenerate, textParameters)} instead.
				 No other figure will gain these effects from the water tiles.
				 """);

			ScenarioRule nextRoundRule = AddScenarioRule("At the end of next round, something will happen");

			await ShowText(
				"As your Void energy rips the Vermling Shaman in half, you feel a strange sensation. The creature’s death appears to have unveiled the cavern entrance, though the uneasy feeling in the back of your mind is growing, expanding.");

			int currentRoundIndex = GameController.Instance.ScenarioPhaseManager.RoundIndex;
			ScenarioEvents.RoundEndedEvent.Subscribe(this,
				parameters => parameters.RoundIndex == currentRoundIndex + 1,
				async _ =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
					nextRoundRule.Remove();

					ScenarioRule shadowRule = AddScenarioRule("Your Shadow will not drop a coin upon death.");

					ScenarioRule shadowDeadRule = AddScenarioRule("Something will happen when Your Shadow is killed.");

					await ShowText(
						"Suddenly, color fades from your vision and the world goes gray. You’re no longer sure where you are. A twisted, shadowy mockery of your form appears in front of you, eyes aglow with Void energy. It glares at you contemptuously and turns to attack.");

					await SpawnMonster(null, ModelDB.Monster<YourShadow>(), MonsterType.Boss,
						GameController.Instance.Map.GetMarker(Marker.Type.a).Hex);

					ScenarioEvents.FigureKilledEvent.Subscribe(this,
						parameters => parameters.Figure is Monster monster && monster.MonsterModel is YourShadow,
						async _ =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
							shadowRule.Remove();
							shadowDeadRule.Remove();

							ScenarioRule ashbladeSpawnRule = AddScenarioRule(textParameters =>
								$"At the end of the round, spawn an elite Aesther Ashblade at {Icons.InlineMarker(Marker.Type.b, textParameters)}. This Ashblade is one level below the scenario level");

							AddScenarioRule("When this Ashblade is slain, your Voidheart is considered cleansed and the scenario is complete.");

							await ShowText("""
							               As the shadowy figure fades away, color floods back into your vision. You can now clearly see the cavern and its spring. You’re not entirely sure what you were fighting, or if it was all in your mind, but your goal lies in ahead of you—finally within your grasp.

							               As you approach the altar in the middle of the spring, you sense a portal opening behind you. An Aesther Ashblade appears in the shadows at the edge of the cavern. This one you recognize—he recruited you as a Hollowpact. He sneers, “You thought you could abandon us? Free yourself? Hilarious.

							               That Void-heart of yours is linked to me— it’s MY power that keeps you alive.” “I felt the moment you brought the stone to this place to defile it. We had granted you a taste of the Void, but now you’re just begging for a full serving.”
							               """);

							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								_ => true,
								async _ =>
								{
									ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

									ashbladeSpawnRule.Remove();

									await SpawnMonster(null, ModelDB.Monster<AestherAshblade>(), MonsterType.Elite,
										GameController.Instance.Map.GetMarker(Marker.Type.b).Hex,
										monsterLevel: GameController.Instance.SavedScenario.ScenarioLevel - 1);

									ScenarioEvents.FigureKilledEvent.Subscribe(this,
										parameters => parameters.Figure is Monster monster && monster.MonsterModel is AestherAshblade,
										async _ =>
										{
											await _goal.AdjustProgress(1);
										});
								});
						});
				});
		}
	}
}