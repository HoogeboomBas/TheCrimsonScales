using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario007 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario007.tscn";
	public override int ScenarioNumber => 7;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<MainCampaignScenarioChain>();
	public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario008>(), new ScenarioConnection<Scenario010>()];

	protected override ScenarioGoals CreateScenarioGoals() => new CustomScenarioGoals("Find all three Golden Eggs to win this scenario.");

	public override string BGSPath => "res://Audio/BGS/Forest Day.ogg";

	private readonly List<(Water, int)> _waterTiles = new List<(Water, int)>();
	private int _lastUsedRoundIndex = -1;

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText(
			$"Once per round, when a character ends their turn on a water tile marked {Icons.Marker(Marker.Type.a)}, " +
			"the water tile is removed and various things might happen, such as monsters spawning and conditions being applied to the character." +
			"\nThe water tiles cannot be removed in any other way.");

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

			if(!GameController.FastForward)
			{
				PopupRequest popupRequest = new TextPopup.Request("Found a Golden Egg", "You found a Golden Egg!");
				AppController.Instance.PopupManager.RequestPopup(popupRequest);

				await GDTask.WaitWhile(AppController.Instance.PopupManager.IsPopupOpen);
			}

			switch(_waterTiles.Count)
			{
				case 8: // 9J
				{
					if(tokenNumber % 2 == 0)
					{
						for(int i = character.Conditions.Count - 1; i >= 0; i--)
						{
							ConditionModel condition = character.Conditions[i];
							if(condition.IsNegative)
							{
								await AbilityCmd.RemoveCondition(character, condition);
							}
						}

						await AbilityCmd.GainXP(character, 5);
					}
					else
					{
						await AbilityCmd.SufferDamage(null, character, HazardousTerrain.DamageAmount);
					}

					break;
				}
				case 4: // 9E
				{
					if(tokenNumber % 2 == 0)
					{
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
					// Gain a random orb item if this scenario has not been completed before
					if(!GameController.Instance.SavedScenarioProgress.Completed)
					{
						ItemModel itemModel = AbilityCmd.GetRandomAvailableOrb();

						await AbilityCmd.GiveItem(character, itemModel);

						GameController.Instance.SavedCampaign.CollectedPartyAchievements.Add(PartyAchievement.FollowTheMoney);
					}

					await ((CustomScenarioGoals)ScenarioGoals).Win();
					break;
				}
			}

			await water.Destroy(forceDestroy: true);

			return;
		}

		Hex hex = water.Hex;
		List<Hex> hexes = RangeHelper.GetHexesInRange(hex, 100, requiresLineOfSight: false).ToList();
		hexes.Shuffle(GameController.Instance.StateRNG);
		hexes.Sort((otherHexA, otherHexB) => RangeHelper.Distance(hex, otherHexA).CompareTo(RangeHelper.Distance(hex, otherHexB)));

		await water.Destroy(forceDestroy: true);

		switch(tokenNumber)
		{
			case 1: // 9D
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Wound1);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 2: // 9I
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Immobilize);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
				}

				break;
			}
			case 3: // 9L
			{
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
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
				}

				break;
			}
			case 4: // 9K
			{
				if(character.Items.Any(item => item.ItemState == ItemState.Spent))
				{
					ItemModel item = await AbilityCmd.SelectItem(character, ItemState.Spent, hintText: "Select an item to refresh");
					await AbilityCmd.RefreshItem(item);
				}

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						break;
					}
				}

				break;
			}
			case 5: // 9H
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Strengthen);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						break;
					}
				}

				break;
			}
			case 6: // 9G
			{
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
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 7: // 9A
			{
				ActionState actionState = new ActionState(character, [HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()]);
				await actionState.Perform();

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 8: // 9C
			{
				await AbilityCmd.InfuseWildElement(character);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<ForestImp>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 9: // 9F
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Muddle);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<SpittingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<Hound>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 10: // 9B
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Bless);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<CaveBear>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						break;
					}
				}

				break;
			}
			case 11: // 9M
			{
				foreach(Character otherCharacter in GameController.Instance.CharacterManager.Characters)
				{
					await AbilityCmd.SufferDamage(null, otherCharacter, 2);

					foreach(Summon summon in otherCharacter.Summons)
					{
						await AbilityCmd.SufferDamage(null, summon, 2);
					}
				}

				break;
			}
			case 12: // 9N
			{
				await AbilityCmd.AddCondition(null, character, Conditions.Curse);

				switch(GameController.Instance.CharacterManager.Characters.Count)
				{
					case 2:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						break;
					}
					case 3:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						break;
					}
					case 4:
					{
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Elite, hexes);
						await SummonMonster(character, ModelDB.Monster<RendingDrake>(), MonsterType.Normal, hexes);
						await SummonMonster(character, ModelDB.Monster<GiantViper>(), MonsterType.Normal, hexes);
						break;
					}
				}

				break;
			}
		}
	}

	private async GDTask SummonMonster(Character character, MonsterModel monsterModel, MonsterType monsterType, List<Hex> hexes)
	{
		Hex chosenHex = await AbilityCmd.SelectHex(character,
			list =>
			{
				Hex firstHex = null;
				foreach(Hex hex in hexes)
				{
					if(hex.IsUnoccupied())
					{
						firstHex = hex;
						break;
					}
				}

				if(firstHex == null)
				{
					return;
				}

				int distance = RangeHelper.Distance(character.Hex, firstHex);

				foreach(Hex otherHex in hexes)
				{
					int otherDistance = RangeHelper.Distance(character.Hex, otherHex);
					if(otherHex.IsUnoccupied() && otherDistance == distance)
					{
						list.Add(otherHex);
					}
				}
			}, true, $"Select where to summon the {monsterType.ToString()} {monsterModel.Name}"
		);

		if(chosenHex == null)
		{
			return;
		}

		await AbilityCmd.SummonMonster(monsterModel, monsterType, chosenHex);
	}
}