using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario049 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario049.tscn";

	public override int ScenarioNumber => 49;
	public override string Name => "Oak Invasion A";

	protected override List<ScenarioRequirement> Requirements => [new PartyAchievementRequirement(PartyAchievement.OaksAlliance, false)];
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Supporting your old friend the Hierophant, you travel to the Dagger Forest to support his brothers in their battle to clear the Inox encampment and reclaim the area for the Oak. You have no great religious beliefs yourself, but you believe in loyalty—and reward for a job well done.

		The central camp is in a wide clearing, accessed by a natural, twisting valley. Suddenly, from nowhere, a large group of Inox ambush your party. It looks like just getting to the contested site will be a challenge.
		""";

	public override string ConclusionText =>
		"""
		The Inox fight bravely, but they are no match for you and the Hierophant’s warriors.

		As you clear the camp, the members of the Oak suddenly fall to their knees around a young sapling you hadn’t previously noticed. As they pray, the edges of its young, delicate leaves begin to glow golden around the edges and you feel infused with strength and good fortune. Maybe there is something to this holy site, after all...
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CityArcher>(),
		ModelDB.Monster<CityGuard>(),
		ModelDB.Monster<InoxArcher>(),
		ModelDB.Monster<InoxBodyguard>(),
		ModelDB.Monster<InoxGuard>(),
		ModelDB.Monster<InoxBodyguard>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(10)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		AddScenarioRule("All City Guards and City Archers are allies to you and to each other and enemies to all other monsters.");

		await ShowText(
			"""
			You burst through the ambush and into the Inox camp. With the element of surprise well and truly lost, they are ready and waiting—and extremely reluctant to give up their camp without a fight.

			A shaman, who seems to be a village elder, steps forward and, quite calmly, says “We had no quarrel with you, until you tried to destroy our homes. Now, you must pay the price.” At this, swords and bows are drawn and the shaman raises his staff. You realize, perhaps a little late, that this group will defend their homes to the death.
			""");
	}
}