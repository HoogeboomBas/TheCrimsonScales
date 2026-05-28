using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario050 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario050.tscn";

	public override int ScenarioNumber => 50;
	public override string Name => "Oak Invasion B";

	protected override List<ScenarioRequirement> Requirements => [new PartyAchievementRequirement(PartyAchievement.InoxAlliance, false)];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Somewhat reluctantly, you support the Inox in their dispute with the Hierophant and his acolytes. You have no strong personal religious beliefs and, although he was a good colleague, you suspect that the Hierophant’s desire for securing a genuine holy site. Besides, the Inox can be powerful allies when needed.

		You are following the directions you have been given to their encampment, when you round a rocky outcrop straight into an ambush.

		“Kill the infidels!” goes a cry from a group wearing the same golden tree insignia as the Hierophant, who also have a ferocious-looking bear under their control. You seem to be close to the Inox camp, but you have a significant obstacle to overcome first.
		""";

	public override string ConclusionText =>
		"""
		The fanatics fight well, but they are no match for you and the Inox warriors defending their camp. As the Inox begin to survey the damage and start to repair their homes, an Inox Shaman approaches you.

		“Thank you,” he says simply with a bow. “You have done us a great service today, and it will not be forgotten.”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaptainOfTheGuard>(),
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<CityArcher>(),
		ModelDB.Monster<CityGuard>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxGuard>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainXPReward(10),
		new GainCheckmarkReward()
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		AddScenarioRule("All Inox Guards and Inox Archers are allies to you and to each other and enemies to all other monsters.");

		await ShowText(
			"""
			Having overcome the ambush, you hurry to the Inox camp. You are just in time—the Hierophant’s acolytes are emerging from the trees, ready to destroy the camp and claim the area for themselves.
			""");
	}
}