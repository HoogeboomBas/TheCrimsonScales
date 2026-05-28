using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario047 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario047.tscn";

	public override int ScenarioNumber => 47;
	public override string Name => "Viper Marsh";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		You’re fairly certain that you’ve followed the Mirefoot’s directions precisely as you were told, but you’ve been wandering through a part of the Lingering Swamp that you never knew existed before today, and the only movement you’ve seen in hours is that of a few Oozes. The Mirefoot isn’t usually wrong about things like this, and if someone in the city can craft a poison from this Ghost Viper venom, then you’re sure it’ll be of value, so you push on.

		Just when you feel ready to give up, you spot it. It’s real. Just behind a pool of shallow water, a Ghost Viper. However, that isn’t the only thing you spot—right before your eyes a demon emerges from the ground below. Life is never simple, is it?

		You will need to incapacitate a few of the Ghost Vipers to quickly grab them and get out of here. Hopefully some make it back to the city alive and can be milked for their venom.
		""";

	public override string ConclusionText =>
		"""
		You grab one last Ghost Viper and make a run for it. It seems some were beaten too badly to survive the trip back to the city, but a few survived.

		After asking around a bit, you find a Quatryl who is able to work with the venom. She offers you a finder’s fee and promises to have a new potent poison on the market in no time.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<GhostViperScenario047>(),
		ModelDB.Monster<Ooze>()
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveItemReward(ModelDB.Item<GhostViperVenom>()),
		new GainProsperityReward(1),
		new GainGoldEachReward(10)
	];

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<GhostViperScenario047>(),
			multiple: true, specificCount: GameController.Instance.SavedCampaign.Characters.Count + 4));

		GameController.Instance.Map.Treasures[0].SetObtainLootFunction(async lootingCharacter =>
		{
			await AbilityCmd.GainGold(lootingCharacter, 15);
		});
	}
}