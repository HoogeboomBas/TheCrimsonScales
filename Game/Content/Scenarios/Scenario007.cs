using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario007 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario007.tscn";

	public override int ScenarioNumber => 7;
	public override string Name => "Golden Eggs";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();

	public override IEnumerable<ScenarioConnection> Connections =>
	[
		new ScenarioConnection<Scenario008>(),
		new ScenarioConnection<Scenario010>()
	];

	public override string IntroductionText =>
		"""
		Keen to impress both Councilman Raksani and Selandre, you hurry to the Lingering Swamp in search of the Golden Eggs. You had heard of the near-mythical Svarn Seahawk, a dangerous and very protective bird which was supposed to lay golden eggs in remote swampland, but you had never believed it personally. However, the directions are precise, and when you get to the site you see a flattened area of marsh with pockmarked, water filled holes, just like the legend tells.

		You still don’t really believe in the Seahawk, but you work quickly, just in case. Otherwise, the place is empty, so this should be a quick recovery job. Nice and easy.
		""";

	public override string ConclusionText =>
		"""
		You return to Selandre with the eggs, reminding yourself that in Gloomhaven, nothing is nice and easy. When you enter the bar, proudly carrying the eggs, it is largely empty. A Quatryl in the corner starts to sing:

		“Our brave adventurers return With just some eggs for dinner They started with such tiny brains And are getting even dimmer!”

		“Ignore her,” says a gruff voice from behind the bar. “Leave them here, I’ll make sure she gets them.”

		“What’s your name?” you ask the Savvas barman in a friendly tone, his scarred chest cavity marking him out as a Cragheart. “And where’s Selandre?”

		“Arrok... and out,” grunts the barman in reply to both questions at once. He takes the eggs, turns his back and walks off. The conversation is clearly over.

		Not particularly wanting to be ignored, or to hear the sequel to the Quatryl’s little ditty, you retire to the Sleeping Lion. A few hours later, you receive a note from a young Vermling pup.

		“Sir R was very pleased with his new acquisitions; reward awaits at the meeting spot. Next job: investigate the shattered fortress, there’s a reward for a missing Quatryl, and I’ve had a tip he’s there or speak to Athan Tredan, Head Keeper of the Gloomhaven Great Oak for more information on the crystal you found. That’s where the real glory lies” .

		Having learned the hard way never to leave money unattended for long, you head back to Selandre’s hideout to collect it before deciding where to go next.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<CaveBear>(),
		ModelDB.Monster<ForestImp>(),
		ModelDB.Monster<GiantViper>(),
		ModelDB.Monster<Hound>(),
		ModelDB.Monster<RendingDrake>(),
		ModelDB.Monster<SpittingDrake>()
	];

	public override List<SavedReward> Rewards =>
	[
		new GainCollectiveGoldReward(30),
		new GainCollectiveRandomOrbReward(),
		new GainPartyAchievementReward(PartyAchievement.FollowTheMoney),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario008>()),
		new UnlockScenarioReward(ModelDB.Scenario<Scenario010>()),
	];

	public override string BGSPath => "res://Audio/BGS/Forest Day.ogg";

	private readonly List<(Water, int)> _waterTiles = new List<(Water, int)>();
	private int _lastUsedRoundIndex = -1;

	private CustomScenarioGoal _goal;

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_goal = await AddGoal(new CustomScenarioGoal(textParameters => "Find 3 Golden Eggs.", hasProgress: true, maxProgress: 3));

		AddScenarioRule(textParameters =>
			$"Once per round, when a character ends their turn on a water tile marked {Icons.InlineMarker(Marker.Type.a, textParameters)}, the water tile is removed and various things might happen, such as monsters spawning and conditions being applied to the character. The water tiles cannot be removed in any other way.");

		List<int> tokenNumbers = new List<int>();
		for(int i = 0; i < 12; i++)
		{
			tokenNumbers.Add(i);
		}

		tokenNumbers.Shuffle(GameController.Instance.StateRNG);

		foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
		{
			if(hex.TryGetHexObjectOfType(out Water water))
			{
				int tokenNumber = tokenNumbers[0];
				tokenNumbers.RemoveAt(0);
				_waterTiles.Add((water, tokenNumber));
			}
		}

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
			parameters => GameController.Instance.ScenarioPhaseManager.RoundIndex > _lastUsedRoundIndex &&
			              parameters.Figure is Character character &&
			              _waterTiles.Any(waterTile => waterTile.Item1.Hex == character.Hex),
			async parameters =>
			{
				Water characterWater = parameters.Figure.Hex.GetHexObjectOfType<Water>();
				if(characterWater == null)
				{
					Log.Error("Something weird happened with the water tiles in this scenario.");
					return;
				}

				_lastUsedRoundIndex = GameController.Instance.ScenarioPhaseManager.RoundIndex;

				int index = _waterTiles.FindIndex((waterTile => waterTile.Item1 == characterWater));
				(Water water, int tokenNumber) = _waterTiles[index];
				_waterTiles.RemoveAt(index);
				//_waterTiles.Clear();
				await SearchWater((Character)parameters.Figure, water, tokenNumber);
			}
		);
	}

	private async GDTask SearchWater(Character character, Water water, int tokenNumber)
	{
		if(_waterTiles.Count % 4 == 0)
		{
			// Looting a golden egg

			await ShowText("Found a Golden Egg", "You found a Golden Egg!");

			await _goal.AdjustProgress(1);

			switch(_waterTiles.Count)
			{
				case 8: // 9J
				{
					if(tokenNumber % 2 == 0)
					{
						await ShowText(textParameters =>
							$"""
							 The character who removed the water tile has all their negative conditions removed and gains {Icons.Inline(Icons.XP, textParameters)}5.
							 """);

						await AbilityCmd.RemoveAllNegativeConditions(character);

						await AbilityCmd.GainXP(character, 5);
					}
					else
					{
						await ShowText(textParameters =>
							$"""
							 The character who removed the water tile suffers {Icons.Inline(Icons.Damage, textParameters)}{HazardousTerrain.DamageAmount}.
							 """);

						await AbilityCmd.SufferDamage(character, HazardousTerrain.DamageAmount, null);
					}

					break;
				}
				case 4: // 9E
				{
					if(tokenNumber % 2 == 0)
					{
						await ShowText(textParameters =>
							$"""
							 All enemies perform “{Icons.Inline(Icons.Heal, textParameters)}3, Self” and add +1{Icons.Inline(Icons.Move, textParameters)} to all their Move abilities this round.
							 """);

						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							if(character.EnemiesWith(figure))
							{
								ActionState actionState = new ActionState(figure,
									[HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()]);
								await actionState.Perform();
							}
						}

						// Add +1 Move to all their Move abilities this round
						ScenarioEvents.AbilityStartedEvent.Subscribe(this,
							parameters => parameters.AbilityState is MoveAbility.State moveAbilityState &&
							              character.EnemiesWith(moveAbilityState.Performer),
							async parameters =>
							{
								MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
								moveAbilityState.AdjustMoveValue(1);

								await GDTask.CompletedTask;
							}
						);

						ScenarioEvents.RoundEndedEvent.Subscribe(this, parameters => true,
							async parameters =>
							{
								ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
								ScenarioEvents.RoundEndedEvent.Unsubscribe(this);

								await GDTask.CompletedTask;
							});
					}
					else
					{
						await ShowText(textParameters =>
							$"""
							 The character who removed the water tile may perform “{Icons.Inline(Icons.Heal, textParameters)}3, Self” and {Icons.Inline(Icons.RecoverCard, textParameters)} two discarded cards.
							 """);

						ActionState actionState =
							new ActionState(character, [HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build()]);
						await actionState.Perform();

						if(character.Cards.Any(card => card.CardState == CardState.Discarded && card.OriginalOwner == character))
						{
							IEnumerable<AbilityCard> cards =
								await AbilityCmd.SelectAbilityCards(character, CardState.Discarded, 1, 2, card => true,
									hintText: "Select up to 2 discarded cards to recover");

							foreach(AbilityCard abilityCard in cards)
							{
								await AbilityCmd.ReturnToHand(abilityCard);
							}
						}
					}

					break;
				}
				case 0: // 6B
				{
					break;
				}
			}

			await water.Destroy(forceDestroy: true);

			return;
		}

		Hex hex = water.Hex;
		// List<Hex> hexes = RangeHelper.GetHexesInRange(hex, RangeHelper.InfiniteRange, requiresLineOfSight: false).ToList();
		// hexes.Shuffle(GameController.Instance.StateRNG);
		// hexes.Sort((otherHexA, otherHexB) => RangeHelper.Distance(hex, otherHexA).CompareTo(RangeHelper.Distance(hex, otherHexB)));

		await water.Destroy(forceDestroy: true);

		switch(tokenNumber)
		{
			case 1: // 9D
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Wound1, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Wound1);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 2: // 9I
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Immobilize, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
				}

				break;
			}
			case 3: // 9L
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 All strong elements are moved to waning.
					 """);

				foreach(Element element in Elements.All)
				{
					if(GameController.Instance.ElementManager.GetState(element) == ElementState.Strong)
					{
						await AbilityCmd.MoveElementToWaning(element);
					}
				}

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
				}

				break;
			}
			case 4: // 9K
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile may {Icons.Inline(Icons.RecoverCard, textParameters)} one spent item.
					 """);

				if(character.Items.Any(item => item.ItemState == ItemState.Spent))
				{
					ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Spent, hintText: "Select an item to refresh");
					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				}

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						break;
					}
				}

				break;
			}
			case 5: // 9H
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Strengthen, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Strengthen);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						break;
					}
				}

				break;
			}
			case 6: // 9G
			{
				await ShowText(textParameters =>
					"""
					Monsters appear!

					The character who removed the water tile discards one hand from hand if possible.
					""");

				if(character.Cards.Any(card => card.CardState == CardState.Hand && card.OriginalOwner == character))
				{
					AbilityCard card = await AbilityCmd.SelectAbilityCard(character, CardState.Hand, true, card => card.OriginalOwner == character,
						hintText: "Select a card to discard");
					await AbilityCmd.DiscardCard(card);
				}

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 7: // 9A
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile may also perform “{Icons.Inline(Icons.Heal, textParameters)}2, Self”.
					 """);

				ActionState actionState = new ActionState(character, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
				await actionState.Perform();

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 8: // 9C
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile may {Icons.InlineWildElement(textParameters)}.
					 """);

				await AbilityCmd.InfuseWildElement(null, character);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 9: // 9F
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Muddle, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Muddle);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 10: // 9B
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Bless, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Bless);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						break;
					}
				}

				break;
			}
			case 11: // 9M
			{
				await ShowText(textParameters =>
					$"""
					 All characters and character summons suffer {Icons.Inline(Icons.Damage, textParameters)}2.
					 """);

				foreach(Character otherCharacter in GameController.Instance.CharacterManager.Characters)
				{
					await AbilityCmd.SufferDamage(otherCharacter, 2, otherCharacter);

					foreach(Summon summon in otherCharacter.Summons)
					{
						await AbilityCmd.SufferDamage(summon, 2, summon);
					}
				}

				break;
			}
			case 12: // 9N
			{
				await ShowText(textParameters =>
					$"""
					 Monsters appear!

					 The character who removed the water tile gains {Icons.InlineCondition(Conditions.Curse, textParameters)}.
					 """);

				await AbilityCmd.AddCondition(null, character, Conditions.Curse);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hex);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hex);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hex);
						break;
					}
				}

				break;
			}
		}
	}
}