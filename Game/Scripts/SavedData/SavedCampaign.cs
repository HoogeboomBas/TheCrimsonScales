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
	public int Reputation { get; private set; }

	[JsonProperty]
	public int Prosperity { get; private set; }

	public event Action CharactersChangedEvent;
	public event Action ProsperityChangedEvent;
	public event Action ReputationChangedEvent;
	public event Action<int> ProsperityLevelChangedEvent;

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

	public static SavedCampaign Test()
	{
		SavedCampaign savedCampaign = New("Party Time", StartingGroup.Militants);

		//savedCampaign.AddCharacter(ModelDB.Class<MirefootModel>(), null, "Moerasvoet");
		//savedCampaign.AddCharacter(ModelDB.Class<BombardModel>(), null, "Beschieter");
		savedCampaign.AddCharacter(ModelDB.Class<HierophantModel>(), ModelDB.PersonalQuest<SpiritualGainsPersonalQuest>(), "Opperpriester");
		//savedCampaign.AddCharacter(ModelDB.Class<FireKnightModel>(), null, "Vuur Knecht");
		//savedCampaign.AddCharacter(ModelDB.Class<ChainguardModel>(), null, "Ketting Garde");
		//savedCampaign.AddCharacter(ModelDB.Class<ChieftainModel>(), null, "Dierenzitter");
		savedCampaign.AddCharacter(ModelDB.Class<AmberAegisModel>(), ModelDB.PersonalQuest<ExperiencedLeader>(), "Sterrenwerper");
		//savedCampaign.AddCharacter(ModelDB.Class<RuinmawModel>(), null, "Ruineerkaak");

		//savedCampaign.Characters[0].AddItem(ModelDB.Item<MinorManaPotion>());
		savedCampaign.Characters[0].SetEquippedSmallSlotItem(0, ModelDB.Item<MinorManaPotion>());
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

	public void UnlockClass(ClassModel classModel)
	{
		string classModelId = classModel.Id.ToString();
		if(!SavedClasses.TryGetValue(classModelId, out SavedClass savedClass))
		{
			savedClass = new SavedClass();
			SavedClasses.Add(classModelId, savedClass);
		}

		savedClass.Unlock();
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

	public void RetireCharacter(SavedCharacter savedCharacter)
	{
		ReturnCards(savedCharacter);

		AdjustProsperity(1);

		Characters.Remove(savedCharacter);
		RetiredCharacters.Add(savedCharacter);

		ClassModel unlockedClass = GetUnlockedClass(savedCharacter);
		if(unlockedClass != null)
		{
			UnlockClass(unlockedClass);
		}

		CharactersChangedEvent?.Invoke();
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
