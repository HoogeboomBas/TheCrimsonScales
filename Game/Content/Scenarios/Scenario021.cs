using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Scenario021 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario021.tscn";

	public override int ScenarioNumber => 21;
	public override string Name => "A Fiery Death";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario027>()];

	public override string IntroductionText =>
		"""
		You cannot wait to show Selandre that you have recovered the Orb of Embers to go with the Frosted Crystal, and burst into The Crimson Scale to show off your find.She is impressed, and a little surprised at your prompt return. “Well, well. How did you find this? Did you get somewhere with that old fool Tredan? He never gives me the time of day.”

		You don’t really know how to answer this, imagining that the grumpy old Master of the Great Oak, selflessly giving his life for others and the arrogant Selandre, always looking for a few more coins, probably have little in common.

		“Well, well,” she says again, slightly lost in thought for a second, before coming back into the present. “OK, here’s what we’ll do next. Let’s start with the Lavalite. Take the Frosted Crystal—it will help when you confront him.” ’Confront him?!’ you think, knowing of his ferocious reputation, which you’re fairly sure Selandre has related on more than one occasion.

		“It’ll be fine,” says Selandre reading your thoughts, “you have proved yourself more than equal to the odd Savvas.” Still dubious, you traverse up to the top of the mountains where the Lavalite is said to reside.

		Sure enough, through the thickness of the trees and boulders, you find a hidden entrance in the side of the mountain. Upon entering, you’re greeted by a Savvas whose body is glowing red like hot coals, and eyes as dark as the midnight sky. “Who do you seek, strangers?”

		You explain that you are looking for the Lavalite on Selandre’s instruction and before you can speak further, the Savvas interrupts you with a scoff. “I kill all who face me, especially those sent by that scheming witch. Prepare to face the wrath of my demons!” The Savvas takes a step back and you notice an Earth Demon appear to your left and a Flame Demon appear to your right. With more on the way, you draw your blade and prepare to eliminate the Savvas once and for all.
		""";

	public override string ConclusionText =>
		"""
		The Lavalite falls to the ground, hopelessly clutching at his fatal blow. “Gloomhaven will fall without us” he croaks. “You have been…” and he falls back, fiery red liquid bubbling out of his mouth and ears.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<EarthDemon>(),
		ModelDB.Monster<FlameDemon>(),
		ModelDB.Monster<Lavalite>(),
	];

	public override List<SavedReward> Rewards =>
	[
		new GainPartyAchievementReward(PartyAchievement.FallenLava),
		//new GainCollectiveItemReward(ModelDB.Item<MagmaWaders>()), //TODO
		new UnlockScenarioReward(ModelDB.Scenario<Scenario027>())
	];

	private Character _frostedCrystalHolder;

	private ScenarioRule _startOfScenarioRule;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		await AddGoal(new KillSpecificEnemyTypeGoal(ModelDB.Monster<Lavalite>()));

		_startOfScenarioRule = AddScenarioRule(textParameters =>
			$"At the start of the scenario, nominate one character to carry the Frosted Crystal.");

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 4).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainGold(character, 15);
			AbilityCard selectedAbilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Lost,
				hintText: $"Select a lost card to {Icons.Inline(Icons.RecoverCard)}");
			if(selectedAbilityCard != null)
			{
				await AbilityCmd.ReturnToHand(selectedAbilityCard);
			}
		});

		GameController.Instance.Map.Treasures.First(treasure => treasure.TreasureNumber == 36).SetObtainLootFunction(async character =>
		{
			await AbilityCmd.GainXP(character, 10, true);
			await AbilityCmd.AddCondition(null, character, Conditions.Invisible);
		});

		ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(this,
			parameters =>
				parameters.Performer == _frostedCrystalHolder &&
				parameters.AbilityState.Target is Monster monster &&
				monster.MonsterModel is Lavalite,
			async parameters =>
			{
				parameters.AbilityState.SingleTargetAdjustAttackValue(1);

				await GDTask.CompletedTask;
			}
		);

		if(GameController.Instance.SavedCampaign.Characters.Count >= 3)
		{
			ScenarioEvents.AbilityStartedEvent.Subscribe(this,
				parameters =>
					parameters.Performer is Monster monster &&
					monster.MonsterModel is Lavalite &&
					parameters.AbilityState is MonsterSummonAbility.State summonAbilityState &&
					(summonAbilityState.MonsterModel is EarthDemon ||
					 (summonAbilityState.MonsterModel is FlameDemon && GameController.Instance.SavedCampaign.Characters.Count >= 4)),
				async parameters =>
				{
					((MonsterSummonAbility.State)parameters.AbilityState).SetMonsterType(MonsterType.Elite);

					await GDTask.CompletedTask;
				}
			);
		}

		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
			parameters =>
				parameters.Figure == _frostedCrystalHolder,
			parameters =>
			{
				parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This character is holding the Frosted Crystal."));
			}
		);
	}

	public override async GDTask OnSetupCompleted()
	{
		await base.OnSetupCompleted();

		_frostedCrystalHolder = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.FirstAlive(), figures =>
		{
			figures.AddRange(GameController.Instance.CharacterManager.Characters);
		}, true, hintText: () => "Select a character to hold the Frosted Crystal") as Character;

		_startOfScenarioRule.Remove();
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
	{
		await base.OnRoomRevealed(parameters);

		if(GameController.Instance.CharacterManager.Characters.Count == 3)
		{
			AddScenarioRule("Every Earth Demon summoned by the Lavalite is elite.");
		}

		if(GameController.Instance.CharacterManager.Characters.Count == 4)
		{
			AddScenarioRule("Every Demon summoned by the Lavalite is elite.");
		}

		AddScenarioRule(textParameters =>
			$"The character holding the Frosted Crystal adds +1{Icons.Inline(Icons.Attack, textParameters)} to all their attacks targeting the Lavalite.");

		await ShowText(
			"""
			You enter the room and the Savvas turns around with a look of shock on his face, as if he’s surprised you’re still alive.

			“That battle was just the appetizer! Prepare yourself for the main course!” with a whisk of hands, more demons step out from the thorns and blazing fires in the room. You look around and take notice that there’s nowhere for the Savvas to go. It’s time to finish this once and for all.
			""");
	}
}