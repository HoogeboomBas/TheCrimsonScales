using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario061 : SoloScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario061.tscn";

	public override int ScenarioNumber => 61;
	public override string Name => "Archaic Preservation";
	public override ClassModel ClassModel => ModelDB.Class<HierophantModel>();
	protected override List<ScenarioRequirement> Requirements { get; } = [new SoloScenarioRequirement(ModelDB.Class<HierophantModel>())];

	public override string IntroductionText =>
		"""
		One of your duties is to maintain the ancient religious artifacts gathered over the years. Although many of your predecessors have been reluctant to care for the old relics, you enjoy the meditative aspect of cleansing and conserving the various idols that have been acquired over the years.

		The centerpiece of the collection is the mighty golem—a huge stone carving, hundreds of years old that, legend has it, comes alive to act as protector of Gloomhaven and those who care for it when needed. Of course, it has remained a statue for as long as you can remember, but you take extra care of it anyway—if nothing else, you appreciate the time and artistry that went into creating the grand idol.

		Late one evening, you are attending to the collection before retiring to bed, when there is an almighty crash, and several slithering figures burst through the large stained-glass window while you hear clicking noises from the back of the room. As the slithering figures’ claws extend and the guns begin to take aim, you are astonished to see the golem’s eyes glow a fiery red and, with a grinding of stone, its fists clench menacingly...
		""";

	public override string ConclusionText =>
		"""
		The terrible crunching of stone on stone ceases as between you and the Golem, you are able to defeat the last of the invaders after a tremendous battle. The fallen Stone Golems lie crushed and broken on the floor and a fine dust fills the air.

		Without pause, the Archaic Golem returns to his podium and, with an almost imperceptible bowing of his head towards you, he assumes his original position before his eyes dim and he returns to his static position, exactly as before. Many mock the Old Fables but, you reflect as you grab a broom and begin to sweep up the debris, those hours of cleaning and polishing the collection certainly paid off tonight.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<AncientArtillery>(),
		ModelDB.Monster<BlackImp>(),
		ModelDB.Monster<Lurker>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<ArchaicGolem>(),
		ModelDB.Monster<StoneGolem>()
	];

	public override List<SavedReward> Rewards =>
	[
		new SoloScenarioReward(ModelDB.Item<RobeOfSoothing>())
	];

	public override async GDTask StartOfScenarioEffects(Character character)
	{
		await base.StartOfScenarioEffects(character);

		await AbilityCmd.AddCondition(null, character, Conditions.Curse);
		await AbilityCmd.AddCondition(null, character, Conditions.Curse);
	}

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		KillAllEnemiesScenarioGoal goal = await AddGoal(new KillAllEnemiesScenarioGoal(enemiesToBeSpawned: true));

		Monster archaicGolem =
			(Monster)GameController.Instance.Map.Figures.First(figure => figure is Monster monster && monster.MonsterModel is ArchaicGolem);

		Hierophant hierophant = (Hierophant)GameController.Instance.CharacterManager.FirstAlive();

		AddScenarioRule(
			"If the Archaic Golem dies, the scenario is lost. You may lose a card to prevent a source of damage on the Archaic Golem.");

		ScenarioEvents.FigureKilledEvent.Subscribe(this, archaicGolem,
			parameters => parameters.Figure == archaicGolem,
			async _ =>
			{
				await AbilityCmd.Lose();
			}
		);

		ScenarioEvents.SufferDamageEvent.Subscribe(this,
			parameters => parameters.Figure == archaicGolem && parameters.WouldSufferDamage && !hierophant.IsDead &&
			              hierophant.Cards.Any(card => card.CardState == CardState.Hand && card.OriginalOwner == hierophant),
			hierophant.LoseCardToCancelDamage, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseCard),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Lose a card from your hand to negate the damage"));

		AddScenarioRule("The Archaic Golem is an ally to you and enemy to all monsters. It draws from your modifier deck.");

		archaicGolem.SetAMDCardDeck(hierophant.AMDCardDeck);

		AddScenarioRule(
			"You can perform actions that allow you to give the Archaic Golem a “Prayer” ability card. Any prayer card can be given to the Archaic Golem with the exception of the card named “Meditation”. At the end of each of its turns, the Archaic Golem can play all its “Prayer” ability cards, performing either the top or bottom action on each card.");

		hierophant.PrayerCards.RemoveAll(abilityCard => abilityCard.Model is Meditation);

		ScenarioCheckEvents.CanBeGivenCardCheckEvent.Subscribe(this,
			parameters =>
			{
				List<AbilityCard> abilityCards = [];
				parameters.GetAbilityCards(abilityCards);
				return parameters.Figure == archaicGolem && abilityCards.Any(card =>
					card.Model.Top is HierophantPrayerCardSide);
			}, parameters =>
			{
				parameters.SetCanBeGivenCard();
			});


		//TODO: Have some visual for what cards he will play/has played
		//TODO: Store this data somewhere so its usable by Restoring Faith
		ScenarioEvents.AbilityCardGivenEvent.Subscribe(this,
			parameters => parameters.CardReceiver == archaicGolem && parameters.AbilityCard.Model.Top is HierophantPrayerCardSide,
			async parameters =>
			{
				ScenarioEvents.FigureTurnEndingEvent.Subscribe(this, parameters.AbilityCard,
					canApplyParameters => canApplyParameters.Figure == archaicGolem,
					async _ =>
					{
						ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(this, parameters.AbilityCard);
						List<CardPlayCardData> cardDatas =
						[
							new CardPlayCardData
							{
								AbilityCard = parameters.AbilityCard,
								CanPlayTop = true,
								CanPlayBottom = true,
								CanPlayBasicTop = true,
								CanPlayBasicBottom = true
							}
						];

						EffectCollection cardSideSelectionEffectCollection =
							ScenarioEvents.CardSideSelectionEvent.CreateEffectCollection(
								new ScenarioEvents.CardSideSelection.Parameters(null));

						AbilityCardSectionSelectionPrompt.Answer cardSectionAnswer = await PromptManager.Prompt(
							new AbilityCardSectionSelectionPrompt(cardDatas, cardSideSelectionEffectCollection,
								() => "Select a card side for the Archaic Golem to perform"), hierophant);

						AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(cardSectionAnswer.CardReferenceId);
						AbilityCardSection section = cardSectionAnswer.AbilityCardSection;

						if(!GameController.FastForward)
						{
							Log.Write($"Playing {card.Model.Name} {section}.");
						}

						switch(section)
						{
							case AbilityCardSection.Top:
								await card.Top.Perform(archaicGolem);
								break;
							case AbilityCardSection.Bottom:
								await card.Bottom.Perform(archaicGolem);
								break;
							case AbilityCardSection.BasicTop:
								await card.BasicTop.Perform(archaicGolem);
								break;
							case AbilityCardSection.BasicBottom:
								await card.BasicBottom.Perform(archaicGolem);
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					});
				await GDTask.CompletedTask;
			});

		ScenarioRule somethingWillHappenRule = AddScenarioRule("Something will happen when all enemies are dead.");

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
			async _ =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);
				await ShowText(
					"Whoever the invaders were: robbers, anarchists or a group trying to bring down the Great Oak, with your support they are no match for the Golem. However, just as the last one is crushed, another group of monsters enter—from all sides this time.");

				await AbilityCmd.SufferDamage(null, archaicGolem, 2);

				await SpawnMonster(null, ModelDB.Monster<Lurker>(), MonsterType.Normal,
					GameController.Instance.Map.GetMarkers(Marker.Type.a).First().Hex);
				await SpawnMonster(null, ModelDB.Monster<Lurker>(), MonsterType.Normal,
					GameController.Instance.Map.GetMarkers(Marker.Type.a).Last().Hex);

				await SpawnMonster(null, ModelDB.Monster<BlackImp>(), MonsterType.Elite,
					GameController.Instance.Map.GetMarkers(Marker.Type.b).First().Hex);
				await SpawnMonster(null, ModelDB.Monster<BlackImp>(), MonsterType.Elite,
					GameController.Instance.Map.GetMarkers(Marker.Type.b).Last().Hex);

				ScenarioEvents.FigureKilledEvent.Subscribe(this,
					_ => KillAllEnemiesScenarioGoal.GetVisibleEnemyCount(true) == 0,
					async _ =>
					{
						ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

						await ShowText(
							"Again, these creatures are no match for the Golem—its red eyes glowing ever brighter, and its movements becoming more agile as it seems to get used to moving its body after centuries of stillness. More creatures enter your Sanctum however, and these are similar to the Archaic Golem itself. Without hesitation though, the Golem raises its mighty fists for one last assault.");

						somethingWillHappenRule.Remove();

						AddScenarioRule("The spawned normal Stone Golems will always act before the Archaic Golem.");

						AddScenarioRule("The Hierophant is now considered to have an initiative of 01 for the purpose of enemy focusing.");

						await AbilityCmd.SufferDamage(null, archaicGolem, 2);

						await SpawnMonster(null, ModelDB.Monster<StoneGolem>(), MonsterType.Normal,
							GameController.Instance.Map.GetMarkers(Marker.Type.a).First().Hex);
						await SpawnMonster(null, ModelDB.Monster<StoneGolem>(), MonsterType.Normal,
							GameController.Instance.Map.GetMarkers(Marker.Type.a).Last().Hex);

						ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(this,
							initiativeCheckParameters => initiativeCheckParameters.Figure == archaicGolem,
							initiativeCheckParameters =>
								initiativeCheckParameters.SetSortingInitiative(initiativeCheckParameters.Initiative.SortingInitiative + 20000));

						ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(this,
							parameters => parameters.PotentialTarget == hierophant,
							parameters =>
							{
								parameters.AdjustTargetSortingInitiative(-hierophant.Initiative.SortingInitiative);
							}
						);

						await goal.DisableEnemiesToBeSpawned();
					});
			});
	}
}