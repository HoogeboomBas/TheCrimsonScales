using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario028 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario028.tscn";

	public override int ScenarioNumber => 28;
	public override string Name => "Fountain of Bones";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario029>()];

	public override string IntroductionText =>
		"""
		You eventually return to the Crimson Scale following your trip to the docks, as directed. Selandre is waiting for you, again looking tense, but she smiles when she sees you.

		“A local village has asked us for help with a little undead problem they’re having. Clear them out, and bring whatever’s ultimately causing it back here and you’ll be well rewarded. How does 70 gold each sound—plus whatever you find along the way of course?” She winks, and her overfriendly demeanor is slightly unsettling, but 70 gold is 70 gold.

		You find the settlement—to call it a village is over-stating it—easily enough, and a shabby-looking elder leads you towards the half-collapsed tomb that is the source of their problems.

		How did these paupers get so much gold together you wonder, before the stench of undead hits you and you realize you’ve found the source of their problems. Several undead turn to face you as you climb down the steps, their swords raised for battle.
		""";

	public override string ConclusionText =>
		"""
		As the fountain crumbles, so do the remaining creatures. Something seems to lift from the air and the sense of creeping death abates.

		The place remains troubled though. You explore the rest of the tomb and find a pile of scrolls in a strange language. Puzzled by what they mean, you take them to the only person you know who likes nothing better than looking at dusty old manuscripts.

		“Friends! You’ve returned!” calls out Dominic from behind his desk at the Town Records office. Leaping up, he knocks over his chair and stumbles over his robes before grasping you all by the hand, beaming widely.

		“Look at my library now!” Dominic exclaims excitedly, “and I must tell you about the next volume of my book!” He pauses in his excitement, just for a second. “But what brings you here, brave adventurers? How can I be of service?” You show Dominic the scrolls and the smile fades from his face, followed by a worried frown.

		“These contain very dark magical instructions and mention a luminescent potion. This enables even those relatively unskilled to engage in necromancy. They refer to another tomb past the Corpsewood which I suggest you investigate.

		”However, and I can’t stress this enough, you cannot let this potion fall into the wrong hands. It should be destroyed forthwith.”

		The scrolls seem to have sapped Dominic of his positivity, so you say farewell without taking the library tour. As you leave, he calls out “Remember, destroy the potion. You must destroy it!”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Cultist>(),
		ModelDB.Monster<LivingBones>(),
		ModelDB.Monster<Ooze>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCheckmarkReward(),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario029>())
	];

	private Door _door2;
	private Objective _fountainOfBones;

	private CustomScenarioGoal _goal;

	private ScenarioRule _doorLockedRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(textParameters => "Destroy the Fountain of Bones.",
			hasProgress: true, maxProgress: 1));

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ManaMedicine>());

		_door2 = GameController.Instance.Map.GetMarker(Marker.Type._2).GetHexObject<Door>();
		_fountainOfBones = GameController.Instance.Map.GetMarker(Marker.Type.a).GetHexObject<Objective>();
		_fountainOfBones.Init(GameController.Instance.SavedCampaign.Characters.Count * (GameController.Instance.SavedScenario.ScenarioLevel + 8),
			"Fountain of Bones");

		ScenarioEvents.FigureKilledEvent.Subscribe(this, _fountainOfBones,
			parameters => parameters.Figure == _fountainOfBones,
			async parameters =>
			{
				await _goal.AdjustProgress(1);
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				canApplyParameters =>
					GameController.Instance.Map.Figures.All(figure => figure.Alignment != Alignment.Monsters),
				async applyParameters =>
				{
					await _door2.Unlock();
					_doorLockedRule.Remove();

					ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
				}
			);

			_doorLockedRule = AddScenarioRule(textParameters => $"The door is locked and can only be opened when all enemies are dead.");
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			AddScenarioRule(textParameters =>
				$"The fountain represents the Fountain of Bones. Whenever a Living Bones is killed, the Fountain of Bones suffers {Icons.Inline(Icons.Damage, textParameters)}{GameController.Instance.SavedCampaign.Characters.Count + 1}.");

			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				canApplyParameters =>
					canApplyParameters.Figure is Monster monster &&
					monster.MonsterModel is LivingBones,
				async applyParameters =>
				{
					await AbilityCmd.SufferDamage(null, _fountainOfBones, GameController.Instance.SavedCampaign.Characters.Count + 1);
				}
			);

			await ShowText(
				"""
				Entering the final chamber of the tomb, you see a bizarre fountain. Fashioned almost entirely out of skulls, along with some other grimly recognizable bones, a green and faintly luminous liquid pours out of the fountain. There seems to be a symbiotic relationship between the fountain and the undead creatures, both energizing each other. You reason that the opposite is true—if you can kill one, you’ll destroy the other.
				""");
		}
	}
}