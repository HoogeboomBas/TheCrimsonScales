using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario044 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario044.tscn";

	public override int ScenarioNumber => 44;
	public override string Name => "Haunted Manor";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<EventScenarioChain>();

	public override string IntroductionText =>
		"""
		Your party approaches the large manor house with little trepidation; tales of ghosts and hauntings have never scared you—even physical beings only occasionally concern you, and that’s usually when they’re trying to kill you.

		In fact, the conversation quickly turns to the other rumors surrounding Shiela’s inheritance—that the old man had numerous treasures dotted all over the place. Reasoning that Shiela had probably not set foot in it after deciding it was haunted, you swear to do a very thorough sweep of the house as you clear the ‘ghosts’ in the hope of finding some trinkets that won’t be missed.

		As you enter the long, sweeping drive however, your jovial mood quickly changes. It is deathly quiet, as if even the birds are afraid to sing here. A strange energy makes the hair on the back of your neck stand up—at least for those of you who have hair (and necks).

		You realize that Shiela didn’t promise you that gold for nothing after all, and you ready yourself to face whatever evil lurks inside.
		""";

	public override string ConclusionText =>
		"""
		As you strike the last of the spirits, it disintegrates with an unearthly howl. Instantly, the strange silence and unnerving energy lift and, although it is now dark outside, the grand old house immediately feels brighter and more homely.

		As you trudge back to The Sleeping Lion to inform Shiela that her inheritance has been secured, you each silently ponder. Despite the loot and odd trinket you collected on the way, not to mention the bag of coins waiting behind the bar, who really got the better deal?
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<HarrowerInfester>(),
		ModelDB.Monster<LivingSpirit>(),
		ModelDB.Monster<NightDemon>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainGoldEachReward(20),
		new GainRandomOrbEachReward()
	];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddConditions(null, character, [Conditions.Curse, Conditions.Curse, Conditions.Curse]);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<LivingSpirit>(),
			specificCount: GameController.Instance.SavedCampaign.Characters.Count * 2));

		GameController.Instance.Map.Treasures[0].SetItemLoot(ModelDB.Item<ConcussionMine>());
		GameController.Instance.Map.Treasures[1].SetItemLoot(AbilityCmd.GetRandomAvailableStone());
		GameController.Instance.Map.Treasures[2].SetItemLoot(ModelDB.Item<DrainingGreaves>());
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(parameters.Room == GameController.Instance.Map.Rooms[1])
		{
			await ShowText(
				"You burst through the door, hoping for some relief from the vengeful spirits. However, this small ante room does not only contain more of the beasts, but a greenish fog, making it hard to focus on your foe.");
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[2])
		{
			await ShowText(
				"Battling on, you find yourself in—a library? A workshop? As the light starts to fade it’s hard to be sure. What you do see though, is yet more ghouls, seeming to gain strength from the ensuing darkness.");
		}

		if(parameters.Room == GameController.Instance.Map.Rooms[3])
		{
			await ShowText(
				"At last, you reach the back of the house and step down to the cellar. Although the tide of spirits is still coming, this feels like their last stand.");
		}
	}
}