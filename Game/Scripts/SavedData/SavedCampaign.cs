using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedCampaign
{
	[JsonProperty]
	public string PartyName { get; set; }

	[JsonProperty]
	public List<SavedCharacter> Characters { get; set; }

	[JsonProperty]
	public SavedScenarioProgresses SavedScenarioProgresses { get; set; }

	[JsonProperty]
	public Dictionary<string, SavedItem> SavedItems { get; set; } // = new Dictionary<string, SavedItem>();

	[JsonProperty]
	public SavedScenario SavedScenario { get; set; }

	public event Action CharactersChangedEvent;

	public static SavedCampaign New(string partyName)
	{
		SavedCampaign savedCampaign = new SavedCampaign()
		{
			PartyName = partyName,
			Characters =
			[
			],
			SavedScenarioProgresses = new SavedScenarioProgresses()
			{
			},
			SavedItems = new Dictionary<string, SavedItem>()
			{
			},
		};

		savedCampaign.SavedScenarioProgresses.ScenarioProgresses.Add(
			ModelDB.GetId<Scenario001>().ToString(),
			new SavedScenarioProgress()
			{
				Discovered = true,
				Unlocked = true,
			}
		);

		ItemModel[] itemModels =
		[
			ModelDB.Item<AmuletOfLife>(),
			ModelDB.Item<CircletOfElements>(),
			ModelDB.Item<HideArmor>(),
			ModelDB.Item<LeatherArmor>(),
			ModelDB.Item<WeatheredBoots>(),
			ModelDB.Item<WingedShoes>(),
			ModelDB.Item<ShoesOfHappiness>(),
			ModelDB.Item<BootsOfSpeed>(),
			ModelDB.Item<HeaterShield>(),
			ModelDB.Item<PoisonDagger>(),
			ModelDB.Item<HookedChain>(),
			ModelDB.Item<IronSpear>(),
			ModelDB.Item<MinorHealingPotion>(),
			ModelDB.Item<MinorPowerPotion>(),
			ModelDB.Item<MinorManaPotion>(),
		];

		foreach(ItemModel itemModel in itemModels)
		{
			savedCampaign.SavedItems.Add(itemModel.Id.ToString(), new SavedItem()
			{
				UnlockedCount = itemModel.ShopCount,
				StockCount = itemModel.ShopCount,
			});
		}

		return savedCampaign;
	}

	public static SavedCampaign Test()
	{
		SavedCampaign savedCampaign = New("Party Time");

		//savedCampaign.AddCharacter(ModelDB.Class<MirefootModel>(), "Swampguy");
		//savedCampaign.AddCharacter(ModelDB.Class<BombardModel>(), "Bombo");
		savedCampaign.AddCharacter(ModelDB.Class<HierophantModel>(), "Conclave Man");
		savedCampaign.AddCharacter(ModelDB.Class<FireKnightModel>(), "Vuur Knecht");

		//savedCampaign.Characters[0].AddItem(ModelDB.Item<MinorManaPotion>());
		savedCampaign.Characters[0].SetEquippedSmallSlotItem(0, ModelDB.Item<MinorManaPotion>());
		//savedCampaign.Characters[1].SetEquippedSmallSlotItem(0, ModelDB.Item<ScrollOfCharisma>());
		//savedCampaign.Characters[1].AddItem(ModelDB.Item<MinorManaPotion>());
		savedCampaign.Characters[0].AddItem(ModelDB.Item<PoisonDagger>());

		return savedCampaign;
	}

	public SavedItem GetSavedItem(ItemModel itemModel)
	{
		if(!SavedItems.TryGetValue(itemModel.Id.ToString(), out SavedItem savedItem))
		{
			savedItem = new SavedItem()
			{
				UnlockedCount = 0,
				StockCount = 0,
			};
			SavedItems.Add(itemModel.Id.ToString(), savedItem);
		}

		return savedItem;
	}

	public void AddCharacter(ClassModel classModel, string name)
	{
		SavedCharacter character = new SavedCharacter(classModel, name);
		Characters.Add(character);

		CharactersChangedEvent?.Invoke();
	}

	public void DeleteCharacter(SavedCharacter savedCharacter)
	{
		// Move all items from this character (back) to the shop
		foreach(string itemId in savedCharacter.ItemIds)
		{
			ItemModel itemModel = ModelDB.GetById<ItemModel>(itemId);
			SavedItem savedItem = GetSavedItem(itemModel);
			savedItem.AddStock(1);
		}

		Characters.Remove(savedCharacter);

		CharactersChangedEvent?.Invoke();
	}
}