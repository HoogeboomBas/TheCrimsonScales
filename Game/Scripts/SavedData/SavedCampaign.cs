using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedCampaign
{
	public static int[] ProsperityLevelThresholds =
	[
		0, // 1
		4, // 2
		9, // 3
		15, // 4
		22, // 5
		30, // 6
		39, // 7
		49, // 8
		59, // 9
	];

	public static int[] ReputationPriceCostThresholds =
	[ // -5
		-18, // -4
		-14, // -3
		-10, // -2
		-6, // -1
		-2, // -0
		3, // 1
		7, // 2
		11, // 3
		15, // 4
		19 // 5
	];

	[JsonProperty]
	public string PartyName { get; private set; }

	[JsonProperty]
	public StartingGroup StartingGroup { get; private set; }

	[JsonProperty]
	public List<SavedCharacter> Characters { get; private set; } = new List<SavedCharacter>();

	[JsonProperty]
	public List<SavedCharacter> RetiredCharacters { get; private set; } = new List<SavedCharacter>();

	[JsonProperty]
	public Dictionary<string, SavedClass> SavedClasses { get; private set; } = new Dictionary<string, SavedClass>();

	[JsonProperty]
	public SavedScenarioProgresses SavedScenarioProgresses { get; private set; }

	[JsonProperty]
	public string CompletedScenarioModelId { get; private set; }

	[JsonProperty]
	public Dictionary<string, SavedItem> SavedItems { get; private set; } = new Dictionary<string, SavedItem>();

	[JsonProperty]
	public SavedScenario SavedScenario { get; private set; }

	[JsonProperty]
	public List<PartyAchievement> CollectedPartyAchievements { get; private set; } = [];

	[JsonProperty]
	public SavedSanctuaryOfTheGreatOak SanctuaryOfTheGreatOak { get; private set; } = new SavedSanctuaryOfTheGreatOak();

	[JsonProperty]
	public SavedEvents SavedEvents { get; private set; } = new SavedEvents();

	[JsonProperty]
	public SavedPersonalQuests SavedPersonalQuests { get; private set; } = new SavedPersonalQuests();

	[JsonProperty]
	public SavedPartyGoals SavedPartyGoals { get; private set; } = new SavedPartyGoals();

	[JsonProperty]
	public SavedMerchantsGuildHall SavedMerchantsGuildHall { get; private set; } = new SavedMerchantsGuildHall();

	[JsonProperty]
	public SavedRewards SavedRewards { get; private set; } = new SavedRewards();

	[JsonProperty]
	public int Reputation { get; private set; }

	[JsonProperty]
	public int Prosperity { get; private set; }

	[JsonProperty]
	public bool EnhancementsUnlocked { get; private set; }

	[JsonProperty]
	public bool GodMode { get; private set; }

	[JsonProperty]
	public Dictionary<string, object> CustomValues { get; private set; } = new Dictionary<string, object>();

	// Collection of ALL characters, even retired and benched ones
	public IEnumerable<SavedCharacter> AllCharacters
	{
		get
		{
			foreach(SavedCharacter character in Characters)
			{
				yield return character;
			}

			foreach(SavedCharacter retiredCharacter in RetiredCharacters)
			{
				yield return retiredCharacter;
			}

			//TODO: Benched characters
		}
	}

	public ScenarioModel CompletedScenarioModel => ModelDB.GetById<ScenarioModel>(CompletedScenarioModelId);

	public event Action CharactersChangedEvent;
	public event Action ProsperityChangedEvent;
	public event Action ReputationChangedEvent;
	public event Action<int> ProsperityLevelChangedEvent;
	public event Action EnhancementsUnlockedChangedEvent;

	public static SavedCampaign New(string partyName, StartingGroup startingGroup)
	{
		SavedCampaign savedCampaign = new SavedCampaign()
		{
			PartyName = partyName,
			StartingGroup = startingGroup,
			Characters =
			[
			],
			SavedScenarioProgresses = new SavedScenarioProgresses()
			{
			},
		};

		ClassModel[] unlockedClassModels = GetStartingClasses(savedCampaign.StartingGroup);
		foreach(ClassModel unlockedClassModel in unlockedClassModels)
		{
			savedCampaign.UnlockClass(unlockedClassModel);
			savedCampaign.SavedPersonalQuests.FilterOutClassPersonalQuests(unlockedClassModel);
		}

		// Unlock the first scenario
		SavedScenarioProgress firstScenario = new SavedScenarioProgress();
		firstScenario.Discover();
		savedCampaign.SavedScenarioProgresses.ScenarioProgresses.Add(ModelDB.GetId<Scenario001>().ToString(), firstScenario);

		// Unlock the first set of items
		savedCampaign.UnlockItems(1);

		return savedCampaign;
	}

	public static SavedCampaign Test(bool godMode = false)
	{
		SavedCampaign savedCampaign = New("Party Time", StartingGroup.Militants);

		//savedCampaign.AddCharacter(ModelDB.Class<MirefootModel>(), null, "Moerasvoet");
		//savedCampaign.AddCharacter(ModelDB.Class<BombardModel>(), ModelDB.PersonalQuest<ExperiencedLeader>(), "Beschieter");
		//savedCampaign.AddCharacter(ModelDB.Class<HierophantModel>(), ModelDB.PersonalQuest<SpiritualGainsPersonalQuest>(), "Opperpriester");
		savedCampaign.AddCharacter(ModelDB.Class<FireKnightModel>(), null, "Vuur Knecht");
		savedCampaign.AddCharacter(ModelDB.Class<ChainguardModel>(), null, "Ketting Garde");
		//savedCampaign.AddCharacter(ModelDB.Class<ChieftainModel>(), null, "Dierenzitter");
		// savedCampaign.AddCharacter(ModelDB.Class<HierophantModel>(), ModelDB.PersonalQuest<AnAdderDivides>(), "Opperpriester");
		//savedCampaign.AddCharacter(ModelDB.Class<SpiritCallerModel>(), null, "Geestroeper");
		// savedCampaign.AddCharacter(ModelDB.Class<HollowpactModel>(), null, "Holle Pakt");
		//savedCampaign.AddCharacter(ModelDB.Class<StarslingerModel>(), ModelDB.PersonalQuest<ExperiencedLeader>(), "Sterrenwerper");
		//savedCampaign.AddCharacter(ModelDB.Class<RuinmawModel>(), null, "Ruineerkaak");

		//savedCampaign.Characters[0].AddItem(ModelDB.Item<MinorManaPotion>());
		savedCampaign.Characters[0].SetEquippedSmallSlotItem(0, ModelDB.Item<FalconFigurine>());
		//savedCampaign.Characters[1].SetEquippedSmallSlotItem(0, ModelDB.Item<ScrollOfCharisma>());
		//savedCampaign.Characters[1].AddItem(ModelDB.Item<MinorManaPotion>());
		savedCampaign.Characters[0].AddItem(ModelDB.Item<PoisonDagger>());
		savedCampaign.Characters[0].AddItem(ModelDB.Item<Chainmail>());
		// savedCampaign.Characters[0].SavedPersonalQuest.PersonalQuestData.AdjustProgress(
		// 	30, savedCampaign.Characters[0].ClassModel, savedCampaign.Characters[0].SavedPersonalQuest.Model);

		savedCampaign.Characters[0].AddGold(1000);

		// SavedScenarioProgress testScenario = new SavedScenarioProgress();
		// testScenario.Discover();
		// savedCampaign.SavedScenarioProgresses.ScenarioProgresses.Add(ModelDB.GetId<Scenario029>().ToString(), testScenario);

		savedCampaign.SetCustomValue("IntroductionSeen", true);

		savedCampaign.GodMode = godMode;

		return savedCampaign;
	}

	public void SetPartyName(string name)
	{
		PartyName = name;
	}

	public void SetSavedScenario(SavedScenario savedScenario)
	{
		SavedScenario = savedScenario;
	}

	public SavedItem GetSavedItem(ItemModel itemModel)
	{
		if(!SavedItems.TryGetValue(itemModel.Id.ToString(), out SavedItem savedItem))
		{
			savedItem = new SavedItem(itemModel);
			SavedItems.Add(itemModel.Id.ToString(), savedItem);
		}

		return savedItem;
	}

	public SavedClass GetSavedClass(ClassModel classModel)
	{
		string classModelId = classModel.Id.ToString();
		if(!SavedClasses.TryGetValue(classModelId, out SavedClass savedClass))
		{
			savedClass = new SavedClass();
			SavedClasses.Add(classModelId, savedClass);
		}

		return savedClass;
	}

	public void UnlockClass(ClassModel classModel)
	{
		SavedClass savedClass = GetSavedClass(classModel);

		if(!savedClass.Unlocked)
		{
			RandomNumberGenerator tempRNG = new RandomNumberGenerator();
			tempRNG.Randomize();
			foreach(EventModel eventModel in classModel.UnlockEvents)
			{
				if(eventModel.EventType == EventType.City)
				{
					SavedEvents.AddCityEventToDeck(eventModel, tempRNG);
				}
				else if(eventModel.EventType == EventType.Road)
				{
					SavedEvents.AddRoadEventToDeck(eventModel, tempRNG);
				}
			}

			savedClass.Unlock();
		}
	}

	public bool CheckClassUnlocked(ClassModel classModel)
	{
		return SavedClasses.TryGetValue(classModel.Id.ToString(), out SavedClass savedClass) && savedClass.Unlocked;
	}

	public void AddCharacter(ClassModel classModel, PersonalQuestModel personalQuestModel, string name)
	{
		SavedCharacter character = new SavedCharacter(classModel, personalQuestModel, name);
		Characters.Add(character);

		CharactersChangedEvent?.Invoke();
	}

	public void DeleteCharacter(SavedCharacter savedCharacter)
	{
		ReturnCards(savedCharacter);

		// Return personal quest
		SavedPersonalQuests.AddPersonalQuest(savedCharacter.SavedPersonalQuest.Model);

		Characters.Remove(savedCharacter);

		CharactersChangedEvent?.Invoke();
	}

	public void RetireCharacter(SavedCharacter savedCharacter, bool addRetirementEvents)
	{
		ReturnCards(savedCharacter);

		AdjustProsperity(1);

		Characters.Remove(savedCharacter);
		RetiredCharacters.Add(savedCharacter);

		if(addRetirementEvents)
		{
			ClassModel classModel = savedCharacter.ClassModel;
			SavedClass savedClass = GetSavedClass(classModel);
			if(!savedClass.Retired)
			{
				RandomNumberGenerator tempRNG = new RandomNumberGenerator();
				tempRNG.Randomize();
				foreach(EventModel eventModel in classModel.RetirementEvents)
				{
					if(eventModel.EventType == EventType.City)
					{
						SavedEvents.AddCityEventToDeck(eventModel, tempRNG);
					}
					else if(eventModel.EventType == EventType.Road)
					{
						SavedEvents.AddRoadEventToDeck(eventModel, tempRNG);
					}
				}

				savedClass.Retire();
			}
		}

		ClassModel unlockedClass = GetUnlockedClass(savedCharacter);
		if(unlockedClass != null)
		{
			UnlockClass(unlockedClass);
		}

		CharactersChangedEvent?.Invoke();
	}

	public void SetCompletedScenario(ScenarioModel scenarioModel)
	{
		CompletedScenarioModelId = scenarioModel?.Id.ToString();
	}

	public void AdjustProsperity(int prosperityAmount)
	{
		int oldProsperityLevel = GetProsperityLevel();
		int oldThresholdProsperityAmount = ProsperityLevelThresholds[Mathf.Min(oldProsperityLevel - 1, ProsperityLevelThresholds.Length - 1)];
		Prosperity += prosperityAmount;
		Prosperity = Mathf.Max(Prosperity, oldThresholdProsperityAmount);

		int newProsperityLevel = GetProsperityLevel();
		if(newProsperityLevel > oldProsperityLevel)
		{
			// New prosperity level, unlock new items
			UnlockItems(newProsperityLevel);
			ProsperityLevelChangedEvent?.Invoke(newProsperityLevel);
		}

		ProsperityChangedEvent?.Invoke();
	}

	public int GetProsperityLevel()
	{
		for(int i = 0; i < ProsperityLevelThresholds.Length; i++)
		{
			int threshold = ProsperityLevelThresholds[i];
			if(threshold > Prosperity)
			{
				return i;
			}
		}

		return ProsperityLevelThresholds.Length;
	}

	public void AddPartyAchievement(PartyAchievement partyAchievement)
	{
		CollectedPartyAchievements.AddIfNew(partyAchievement);
	}

	public bool HasPartyAchievement(PartyAchievement partyAchievement)
	{
		return CollectedPartyAchievements.Contains(partyAchievement);
	}

	public void AdjustReputation(int reputationAmount)
	{
		Reputation += reputationAmount;
		Reputation = Mathf.Clamp(Reputation, -20, 20);

		ReputationChangedEvent?.Invoke();
	}

	public int GetReputationThresholdIndex()
	{
		for(int i = 0; i < ReputationPriceCostThresholds.Length; i++)
		{
			int threshold = ReputationPriceCostThresholds[i];
			if(threshold > Reputation)
			{
				return i;
			}
		}

		return ReputationPriceCostThresholds.Length;
	}

	public int GetReputationItemPriceChange()
	{
		int thresholdIndex = GetReputationThresholdIndex();
		return 5 - thresholdIndex;
	}

	public static ClassModel[] GetStartingClasses(StartingGroup startingGroup)
	{
		return startingGroup switch
		{
			StartingGroup.Militants =>
			[
				ModelDB.Class<BombardModel>(),
				ModelDB.Class<FireKnightModel>(),
				ModelDB.Class<HierophantModel>(),
				ModelDB.Class<MirefootModel>()
			],
			StartingGroup.Protectors =>
			[
				ModelDB.Class<ChainguardModel>(),
				ModelDB.Class<ChieftainModel>(),
				ModelDB.Class<FireKnightModel>(),
				ModelDB.Class<HierophantModel>()
			],
			StartingGroup.Explorers =>
			[
				ModelDB.Class<BrightsparkModel>(),
				ModelDB.Class<ChainguardModel>(),
				ModelDB.Class<HollowpactModel>(),
				ModelDB.Class<StarslingerModel>()
			],
			StartingGroup.Trailblazers =>
			[
				ModelDB.Class<BombardModel>(),
				ModelDB.Class<BrightsparkModel>(),
				ModelDB.Class<LuminaryModel>(),
				ModelDB.Class<StarslingerModel>()
			],
			StartingGroup.Naturalists =>
			[
				ModelDB.Class<ChieftainModel>(),
				ModelDB.Class<HollowpactModel>(),
				ModelDB.Class<LuminaryModel>(),
				ModelDB.Class<MirefootModel>()
			],
			_ => throw new ArgumentOutOfRangeException(nameof(startingGroup), startingGroup, null)
		};
	}

	public ClassModel GetUnlockedClass(SavedCharacter savedCharacter)
	{
		SavedPersonalQuest savedPersonalQuest = savedCharacter.SavedPersonalQuest;
		if(savedPersonalQuest != null &&
		   savedPersonalQuest.Model.ClassToUnlock != null &&
		   !CheckClassUnlocked(savedPersonalQuest.Model.ClassToUnlock))
		{
			return savedPersonalQuest.Model.ClassToUnlock;
		}

		return null;
	}

	public void UnlockEnhancements()
	{
		if(EnhancementsUnlocked)
		{
			return;
		}

		EnhancementsUnlocked = true;
		EnhancementsUnlockedChangedEvent?.Invoke();
	}

	public void SetCustomValue(string key, object value)
	{
		CustomValues[key] = value;
	}

	public T GetCustomValue<T>(string key)
	{
		if(!CustomValues.TryGetValue(key, out object value))
		{
			return default;
		}

		if(value is not T castValue)
		{
			Log.Error($"Could not cast custom value for: {key}");
			return default;
		}

		return castValue;
	}

	public bool TryGetCustomValue<T>(string key, out T value)
	{
		if(!CustomValues.TryGetValue(key, out object retrievedValue))
		{
			value = default;
			return false;
		}

		if(retrievedValue is not T castValue)
		{
			Log.Error($"Could not cast custom value for: {key}");
			value = default;
			return false;
		}

		value = castValue;
		return true;
	}

	private void UnlockItems(int prosperityLevel)
	{
		ItemModel[] itemModels = ItemCollections.Levels[prosperityLevel - 1];
		foreach(ItemModel itemModel in itemModels)
		{
			SavedItem savedItem = GetSavedItem(itemModel);
			int currentlyUnlockedCount = savedItem.UnlockedCount;
			savedItem.AddUnlocked(itemModel.ShopCount - currentlyUnlockedCount);
			savedItem.AddStock(itemModel.ShopCount - currentlyUnlockedCount);
		}

		if(prosperityLevel > 1)
		{
			AppController.Instance.PopupManager.RequestPopup(new ProsperityLevelUpPopup.Request()
			{
				Level = prosperityLevel,
				ItemModels = itemModels
			});
		}
	}

	private void ReturnCards(SavedCharacter savedCharacter)
	{
		// Move all items from this character (back) to the shop
		foreach(string itemId in savedCharacter.ItemIds)
		{
			ItemModel itemModel = ModelDB.GetById<ItemModel>(itemId);
			SavedItem savedItem = GetSavedItem(itemModel);
			savedItem.AddStock(1);
		}

		// Return temporary AMD cards
		SanctuaryOfTheGreatOak.ReturnCards(savedCharacter);

		// Unsubscribe personal quest events
		savedCharacter.SavedPersonalQuest?.Model.OnBetweenScenariosEnded(savedCharacter);
	}
}