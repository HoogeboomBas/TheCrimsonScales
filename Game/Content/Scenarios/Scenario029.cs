using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario029 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario029.tscn";

	public override int ScenarioNumber => 29;
	public override string Name => "Burial Chamber";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<TaintedScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario030>(true)];

	public override string IntroductionText =>
		"""
		With Dominic’s warning ringing in your ears, you follow the directions he interpreted to the chamber referenced in the scrolls. It is not too far from the city center, and after a short search in some deep undergrowth, you find a flight of half-buried steps set into the ground. Lighting torches, you venture down.

		As you reach the bottom, the same foul stench hits your nostrils. You are still adjusting to the smell and the relative darkness when a hail of arrows flies towards you from the distant gloom.

		You manage to duck and block all the arrows but you know there will be more volleys following soon. Meanwhile, something rather close is shambling towards you.
		""";

	public override string ConclusionText =>
		"""
		You slay the last of the Bone Archers and breathe a sigh of relief that their deadly arrows have fallen silent. The shambling undead are relatively easy to deal with, but you remain concerned at the source of this infestation. Again, there is the eerie, green and faintly luminous liquid on both the undead and spattered around the chamber, particularly in one corner. After some cautious investigation of the area, you realize that rather than the wall you had assumed it to be, it is in fact a collapsed corridor leading further into the chamber network.

		Heaving the fallen rocks aside, you make a gap big enough to crawl through to whatever lies beyond.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BoneArcher>(),
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<LivingSpirit>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario030>())
	];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddConditions(null, character, [Conditions.Curse, Conditions.Curse, Conditions.Curse]);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<BoneArcher>(), true));

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ChainMace>());
	}
}