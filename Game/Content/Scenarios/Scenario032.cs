using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

public class Scenario032 : ScenarioModel
{
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class Scenario032Reward : SavedReward
	{
		public override RewardType Type => RewardType.Immediate;

		public Scenario032Reward()
		{
		}

		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Completed the Main Campaign!";

		public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
		{
			await base.ImmediateResolve(savedCampaign, cancellationToken);

			AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Congratulations!",
				"""
				You've beaten the Main Campaign! Thank you so much for playing, we hope you enjoyed it.

				If you have any thoughts, questions or feedback, please let us know!

				For more information about the game, please visit: https://www.thecrimsonscales.com/
				"""));

			await GDTask.WaitWhile(() => AppController.Instance.PopupManager.IsPopupOpen<TextPopup.Request>(), cancellationToken: cancellationToken);
		}
	}

	public override string ScenePath => "res://Content/Scenarios/Scenario032.tscn";

	public override int ScenarioNumber => 32;
	public override string Name => "Confronting the Past";

	public override List<ScenarioLink> Links => [GloomhavenLink.Instance];

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		You step into The Crimson Scale and pause. Something is different; there is a tension in the air, as if battle lines have been drawn.

		Arrok the barman, a loose group of other mercenaries and the singing Quatryl (of course) stand in a group facing you with Selandre at their head. You also notice Sankas cowering in the background.

		“Hello, friends” purrs Selandre disingenuously. “So glad you popped by…” “What’s going on?” you ask, confused. You came here for a drink, not a fight.

		“You still don’t get it, do you?” Selandre says, the smile gone now. “Do you really think I wanted you for your abilities?” she laughs, as do her various cronies. “I’ve spent the last six months trying to kill you off—though you have been very helpful to me.”

		On seeing your confusion, she carries on. “There were many threats to my domination of this town, and you’ve eliminated most of them, albeit mainly through good fortune and stupidity.”

		“Gaining the Orb of Embers and The Frosted Crystal for me, then using them to kill The Lavalite and the Icebound, was very helpful, as was disrupting the Aesther’s attempt to bio-engineer creatures to stop me. But best of all was your ‘rescue’ of my friend Sankas here—his weaponry skills have been most useful.” At this, one of Selandre’s henchmen wheels out a fearsome looking piece of artillery, glowing with the energy stone you rescued. Sankas looks distinctly ashamed, and scuttles off out of sight. Still completely taken aback by what you thought was a business arrangement, you only manage to utter “wha-why?”

		“Why?!” answers Selandre incredulously. “You mean you still don’t get it? Look around; you and your predecessors have been responsible for the slaughter of innocents, our friends, our… family!”

		And now you see it. Now you understand. Now you recognize the resemblance, even before Selandre shakes down her red hair. “Jekserah” you say, almost to yourself.

		“Don’t you mention my sister’s name!” Selandre screams in shock. “It isn’t just about her, although someone killed her, and I know you were associates of hers. We are The Crimson Scales, and we came together to wreak bloody justice for all the people you… self-appointed militia have taken from us. Now, we will have our REVENGE!”
		""";

	public override string ConclusionText =>
		"""
		Having fought for your life, you manage to overpower the last of Selandre’s horde. You have mixed feelings—it felt good to be in an elite group of mercenaries and now it turns out it was a trick all along. Still, as the last ones standing, you must be the elite, the best of the best—though your aching bodies disagree.

		Despite seemingly inheriting a bar of your own (which you now embarrassingly see was called The Crimson Scales all along), you know where you’re going. First you carefully pack the Frosted Crystal, The Orb of Embers and The Book of Naiqa and drop them at the Sanctuary, asking Athan Tredan to act as custodian of these precious, but dangerous artifacts.

		Then, you walk out of the door, and cross the road—there’s a dark corner of The Sleeping Lion with your name on it.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtillery>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxBodyguardScenario032>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<LivingBones>(),
		ModelDB.Monster<Selandre>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new Scenario032Reward()
	];

	public Monster AncientArtillery { get; private set; }
	public Marker MarkerB { get; private set; }

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<Selandre>()));

		MarkerB = GameController.Instance.Map.GetMarker(Marker.Type.b);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomRevealedParameters)
	{
		await base.OnRoomRevealed(roomRevealedParameters);

		if(roomRevealedParameters.Room == GameController.Instance.Map.Rooms[2])
		{
			AddScenarioRule("The Ancient Artillery cannot be targeted by enemies, and does not take turns.");

			AncientArtillery = GameController.Instance.Map.GetMarker(Marker.Type.z).Hex.GetHexObjectOfType<Monster>();

			ScenarioCheckEvents.CanTakeTurnCheckEvent.Subscribe(this, AncientArtillery,
				parameters =>
					parameters.Figure == AncientArtillery,
				parameters =>
				{
					parameters.SetCannotTakeTurn();
				}
			);

			ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(this, AncientArtillery,
				parameters =>
					parameters.PotentialTarget == AncientArtillery &&
					parameters.Performer.EnemiesWith(AncientArtillery),
				parameters =>
				{
					parameters.SetCannotBeFocused();
				}
			);

			ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, AncientArtillery,
				parameters =>
					parameters.PotentialTarget == AncientArtillery &&
					parameters.Performer.EnemiesWith(AncientArtillery),
				parameters =>
				{
					parameters.SetCannotBeTargeted();
				}
			);

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this, AncientArtillery,
				parameters =>
					parameters.Performer is Monster monster &&
					monster.MonsterModel is Selandre &&
					monster.TakingTurn &&
					parameters.AbilityState.Performed &&
					parameters.AbilityState.Authority == parameters.Performer &&
					parameters.AbilityState is AttackAbility.State,
				async parameters =>
				{
					if(parameters.Performer is Monster selandre)
					{
						// Take Selandre's drawn card and give it to the Ancient Artillery to perform
						ActionState actionState = new ActionState(parameters.AbilityState.ActionState, AncientArtillery, [
							selandre.MonsterGroup.ActiveMonsterAbilityCard.Model.GetAbilities(AncientArtillery)
								.Select(ability => ability.Ability).First(ability => ability is AttackAbility)
						]);

						await actionState.Perform();
					}
				}
			);
		}
	}
}