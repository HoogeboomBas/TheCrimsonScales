using System.Collections.Generic;
using Fractural.Tasks;

public class Scenario013 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario013.tscn";

	public override int ScenarioNumber => 13;
	public override string Name => "Corpse Cavern";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override string IntroductionText =>
		"""
		You are walking through the Sinking Market, trying to avoid the hustlers hawking strange pieces of metal and the like, when you are approached by a slightly flustered Shiela, the potion maker from The Sleeping Lion.

		“Just the people!” she cries out, “I could really do with a favor, if you aren’t too busy.”

		Always keen to help someone so generous with her useful concoctions, you ask her to explain what you can do to help.

		“There is a cavern, not too far from here, which is the only place for miles around that Hanging Moss can be found. This is a most useful ingredient in many of my more advanced potions, but it requires very delicate handling. Unfortunately, my stocks are running low and the cave is populated by some horrible creatures. Could you clear the cavern for me, so that I can harvest the moss?

		“I will give you the choice of my next batch of elixirs,” she adds with a wink. Stepping into a cave full of ‘horrible creatures’ doesn’t sound like enormous fun, but Shiela’s concoctions are the best in the business.

		As you enter the cavern, you start to doubt the wisdom of this trip almost immediately, as you are greeted by shambling re-animated corpses who head for you as soon as you enter the chamber. This had better be the most amazing moss in Gloomhaven.
		""";

	public override string ConclusionText =>
		"""
		Entering the final chamber, the entrance way littered with the bodies of a variety of creatures, you see long, waving tendrils of moss hanging from the ceiling, as well as a few more creatures attempting to kill you. Having dealt with them, you return to look at the moss and gingerly reach out to touch it. It recoils for a second, before detaching itself from the ceiling and wrapping itself around you with a strangling intensity. With some difficulty, you fend it off and retreat to Gloomhaven, letting Shiela know that it is safe to enter the cavern.

		A few days later she sees you again. Looking at the weals the hanging moss has left, she smiles. “I hope I didn’t put you in any trouble,” she says, slightly mischievously. “That moss has incredible powers—help yourself to one of these, and thank you!”
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<LivingCorpse>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<Ooze>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainProsperityReward(1),

		//TODO: Not the true reward for this scenario
		new GainCollectiveItemReward(ModelDB.Item<MajorStaminaPotion>()),
		new GainCollectiveItemReward(ModelDB.Item<MajorPowerPotion>()),
		new GainCollectiveItemReward(ModelDB.Item<MajorManaPotion>()),
	];

	public override string BGSPath => "res://Audio/BGS/Cave.ogg";

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await AbilityCmd.AddCondition(null, character, Conditions.Poison1);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillAllEnemiesScenarioGoal());
		AddScenarioRule(textParameters => $"All Living Corpses add {Icons.Inline(Icons.Targets, textParameters)} 1 on all their attacks.");

		GameController.Instance.Map.Treasures[0].SetItemLoot(AbilityCmd.GetRandomAvailableStone());

		ScenarioEvents.AbilityStartedEvent.Subscribe(this,
			parameters =>
				parameters.AbilityState is AttackAbility.State &&
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;
				attackAbilityState.AdjustTargets(1);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(this,
			parameters =>
				parameters.Performer is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			parameters =>
			{
				parameters.AdjustTargets(1);
			}
		);

		ScenarioCheckEvents.TargetsCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure is Monster monster &&
				monster.MonsterModel == ModelDB.Monster<LivingCorpse>(),
			parameters =>
			{
				parameters.AdjustTargets(1);
			}
		);
	}
}