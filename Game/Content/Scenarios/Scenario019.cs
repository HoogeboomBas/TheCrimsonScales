using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario019 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario019.tscn";

	public override int ScenarioNumber => 19;
	public override string Name => "Elemental Experiments";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<WondrousScenarioChain>();

	public override IEnumerable<ScenarioConnection> Connections =>
	[
		new ScenarioConnection<Scenario023>(), new ScenarioConnection<Scenario024>(), new ScenarioConnection<Scenario025>()
	];

	public override string IntroductionText =>
		"""
		Following the notes you found in the Den of Monstrosity that Athan Tredan had directed you to, you head to the nearby building which was mentioned in the jotted records you discovered.

		As well as copious notes about harvesting and altering the elemental powers of various demons, often written on top of other notes, there are strange runes and symbols that you don’t understand. Although you quite enjoy decoding such mysteries—hoping to unlock secrets—others in your group have no time for them, and see them as a distraction.

		It is without quite knowing the full picture therefore, that you quietly break the lock and enter the building. Whatever you were expecting, this isn’t it.

		A fully equipped laboratory stands in front of you with a variety of strange devices, many emitting an intense hum and a fierce static charge. On one wall, there is a large device that appears to be harvesting mana—for there are also six tubes with the elemental signs above, and filled to different levels with a gas, or maybe a liquid, flowing into them.

		The source of this mana generation is equally astounding—around the edges of the room are various demons and Vermlings held off the ground in some kind of energy bubble. These bubbles also crackle and hum with a barely contained force.

		Just as you are taking this all in, an Aesther walks through the door. Freezing at the sight of you for a second, she recovers her poise and makes a smooth but complicated gesture with her hands. You brace yourself for some form of psychic attack, when she opens a rift and steps through it, disappearing to another realm. As she goes, the machines fall silent, and the energy bubbles that are holding the demons captive also fades away.

		As the (fairly angry) demons fall to the ground, you accidentally step onto a plate on the floor and the elemental tubes drain of their contained mana. This would be interesting, if there were not several demons heading your way, who seem to blame you for their captivity.
		""";

	public override string ConclusionText =>
		"""
		Still in awe of this strange—and slightly horrifying—place, you investigate the Aesther’s lab. You discover further information about what she was working on. It looks like she has been studying the ice element in particular, and has discovered (or created?) a new group of frozen creatures, and is in contact with a fellow master of elements.

		Deeply troubled by the idea of new and unknown demons, you resolve to seek out and destroy them before they multiply, and to track down this other elemental master. In the meantime, you help yourself to some useful looking potion and move on.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<FrostDemon>(),
		ModelDB.Monster<NightDemon>(),
		ModelDB.Monster<SunDemon>(),
		ModelDB.Monster<VermlingExperiment>(),
		ModelDB.Monster<WindDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<MajorManaPotion>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario023>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario024>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario025>()),
	];

	private IEnumerable<PressurePlate> _pressurePlates;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
		AddScenarioRule(textParameters =>
			$"If any character ends their turn on a pressure plate, they may consume {Icons.Inline(Icons.WildElement, textParameters)} X times, where X is the number of Vermling Experiments in the room.");

		_pressurePlates = GameController.Instance.Map.GetMarkers(Marker.Type.a).Select(marker => marker.GetHexObject<PressurePlate>());

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<InfraredGoggles>());
		GameController.Instance.Map.Treasures[1].SetObtainLootFunction(async character =>
		{
			await new ActionState(character, [HealAbility.Builder().WithHealValue(6).WithTarget(Target.Self).Build()]).Perform();
			await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
		});

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => parameters.Figure is Character &&
			              _pressurePlates.Select(pressurePlate => pressurePlate.Hex).Contains(parameters.Figure.Hex),
			async parameters =>
			{
				int count = parameters.Figure.Hex.Room.Figures.Count(
					figure => figure is Monster monster && monster.MonsterModel is VermlingExperiment);
				for(int i = 0; i < count; i++)
				{
					if(await AbilityCmd.AskConsumeWildElement(parameters.Figure) == null)
					{
						break;
					}
				}
			});
	}
}