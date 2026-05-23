using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario014 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario014.tscn";

	public override int ScenarioNumber => 14;
	public override string Name => "Cultist Cave";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		You are in the Sleeping Lion, catching up with Shiela and her latest potion developments when the singing Quatryl appears. The other patrons are quite happy at this—they’ll either get a good song, or an opportunity to heckle—but you fail to hide your disgust.

		Enjoying the attention, she skips over to your table, before making an enormous show of presenting a rolled-up note to you:

		“I have received word of a group of zealots meeting in secret just outside the city. They are currently small in number, but are actively recruiting—including one of our own. It is likely that they have some mastery of necromancy, so expect some undead creatures, but there are also likely to be rewards. Keep what you find, but just eliminate the 3 ringleaders. They are meeting tonight. Go straight to the caves marked on the map below and you will find them there—with no other way out.”

		You pass the note around, nod in agreement, drink up and go—still ignoring the singing Quatryl.

		There’s a drunken heckle from the far corner at your sudden leaving, and a cry of “Give us a song, bard!” You make for the door, but as you turn to open it, you see the singing Quatryl standing on your table, guitar in hand.

		As the door closes behind you, you hear her start to sing: “Killing gruesome undead hordes Our heroes dare to go The only trouble they will have Is telling friend from foe”

		You spend your trip to the caves debating how angry Selandre will be when the Quatryl disappears, and imagining increasingly gruesome ways to end her singing career.

		As you reach the place marked on the map, you realize it’s less of an entrance, more of a hole. There’s a low glow coming from the hole, as if lit by torchlight. This is the place.

		Not believing in the ‘look before you leap’ mantra, you jump down—only to find yourself surrounded by monsters. Maybe there is wisdom in the saying after all.
		""";

	public override string ConclusionText =>
		"""
		You step over the last of the bodies, counting your loot, and then look at the hole you dropped through, some ten feet above your heads. Maybe a little more planning wouldn’t go amiss next time…
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<Cultist>(),
		ModelDB.Monster<DeepTerror>(),
		ModelDB.Monster<LivingCorpse>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(15)
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	private CustomScenarioGoal _goal;
	private ScenarioRule _somethingWillHappenRule;

	private bool _firstDoorOpened;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<DizzyingTincture>());
		GameController.Instance.Map.Treasures[1].SetItemLoot(ModelDB.Item<OrbOfFortune>());

		_goal = await AddGoal(new CustomScenarioGoal(textParameters => "Kill 3 Cultists.", hasProgress: true, maxProgress: 3));

		_somethingWillHappenRule = AddScenarioRule("Something will happen once all enemies in this room are killed.");

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(!_firstDoorOpened)
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure.Alignment == Alignment.Monsters)
						{
							return;
						}
					}

					_somethingWillHappenRule.Remove();

					Door firstDoor = GameController.Instance.Map.GetMarker(Marker.Type._1).Hex.GetHexObjectOfType<Door>();
					await firstDoor.Open(null);

					await ShowText(
						"With corpses scattered across the ground, you hear rumbling and take notice of a large stone being removed from what must be a doorway. A hooded figure enters and lets out a small shriek at the sight of your presence. He quickly turns and runs back through the newly revealed doorway. That must be one of the cultists, and you’ll see to it that was the last time he’ll ever shriek.");

					AddScenarioRule("Whenever a cultist performs a summon ability, it summons a Living Corpse instead of a Living Bones.");

					_firstDoorOpened = true;
				}

				if(parameters.Figure is Monster monster && monster.MonsterModel == ModelDB.Monster<Cultist>())
				{
					await _goal.AdjustProgress(1);
				}
			}
		);

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters =>
				parameters.AbilityState is MonsterSummonAbility.State &&
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<Cultist>(),
			async parameters =>
			{
				MonsterSummonAbility.State monsterSummonAbilityState = (MonsterSummonAbility.State)parameters.AbilityState;
				monsterSummonAbilityState.SetMonsterModel(ModelDB.Monster<LivingCorpse>());

				await GDTask.CompletedTask;
			}
		);
	}
}