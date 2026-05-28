using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedCharacter
{
	[JsonProperty]
	public string ClassModelId { get; private set; }

	[JsonProperty]
	public int Level { get; private set; }

	[JsonProperty]
	public bool LevelUpInProgress { get; private set; }

	[JsonProperty]
	public List<SavedAbilityCard> AvailableAbilityCards { get; private set; }

	[JsonProperty]
	public List<int> HandAbilityCardIndices { get; private set; }

	[JsonProperty]
	public int Gold { get; private set; }

	[JsonProperty]
	public int XP { get; private set; }

	[JsonProperty]
	public string Name { get; private set; }

	[JsonProperty]
	public List<string> ItemIds { get; private set; } = new List<string>();

	[JsonProperty]
	public string[] EquippedBaseSlotItems { get; private set; }

	[JsonProperty]
	public List<string> EquippedSmallItems { get; private set; }

	[JsonProperty]
	public int TotalAvailablePerkCount { get; private set; }

	[JsonProperty]
	public List<int> AcquiredPerkIndices { get; private set; } = new List<int>();

	[JsonProperty]
	public string[] DonationAMDCardIds { get; private set; }

	[JsonProperty]
	public int CheckmarkCount { get; private set; }

	[JsonProperty]
	public SavedPersonalQuest SavedPersonalQuest { get; private set; }

	[JsonProperty]
	public Guid Guid { get; private set; } = Guid.NewGuid();

	public ClassModel ClassModel => ModelDB.GetById<ClassModel>(ClassModelId);

	public event Action<SavedCharacter> GoldChangedEvent;
	public event Action<SavedCharacter> XPChangedEvent;
	public event Action<SavedCharacter> LevelChangedEvent;
	public event Action<SavedCharacter> NameChangedEvent;
	public event Action<SavedCharacter> EquipmentChangedEvent;
	public event Action<SavedCharacter> CardsChangedEvent;
	public event Action<SavedCharacter> CheckmarkCountChangedEvent;
	public event Action<SavedCharacter> PerksChangedEvent;

	public SavedCharacter()
	{
	}

	public SavedCharacter(ClassModel classModel, PersonalQuestModel personalQuestModel, string name = "")
	{
		ClassModelId = classModel.Id.ToString();
		Level = 1;
		AvailableAbilityCards = classModel.AbilityCards
			.Where(abilityCardModel => abilityCardModel.Level == 1)
			.Select(abilityCardModel => new SavedAbilityCard(abilityCardModel))
			.ToList();

		HandAbilityCardIndices = new List<int>();
		for(int i = 0; i < classModel.HandSize; i++)
		{
			HandAbilityCardIndices.Add(i);
		}

		Gold = 30;
		Name = name;
		EquippedBaseSlotItems = new string[5];
		EquippedSmallItems = new List<string>();

		if(personalQuestModel != null)
		{
			SavedPersonalQuest = new SavedPersonalQuest(personalQuestModel);
		}
	}

	public void AddGold(int gold)
	{
		Gold += gold;

		GoldChangedEvent?.Invoke(this);
	}

	public void RemoveGold(int gold)
	{
		Gold -= gold;

		GoldChangedEvent?.Invoke(this);
	}

	public void AddXP(int xp)
	{
		XP += xp;

		XPChangedEvent?.Invoke(this);
	}

	public bool CheckCanLevelUp()
	{
		if(LevelUpInProgress)
		{
			return false;
		}

		if(Level - 1 < ClassModel.XPLevelValues.Values.Length)
		{
			int nextLevelXP = ClassModel.XPLevelValues.Values[Level - 1];
			if(XP >= nextLevelXP)
			{
				return true;
			}
		}

		return false;
	}

	public void TryLevelUp()
	{
		if(CheckCanLevelUp())
		{
			Level++;
			LevelUpInProgress = true;
			AddAvailablePerk();

			LevelChangedEvent?.Invoke(this);
		}
	}

	public void AddLevelUpCard(AbilityCardModel abilityCardModel)
	{
		if(AvailableAbilityCards.Any(card => card.Model == abilityCardModel))
		{
			Log.Error("Trying to add a card that was added earlier.");
			return;
		}

		if(!LevelUpInProgress)
		{
			Log.Error("Trying to add a card while this character is not leveling up currently.");
			return;
		}

		AvailableAbilityCards.Add(new SavedAbilityCard(abilityCardModel));

		LevelUpInProgress = false;

		CardsChangedEvent?.Invoke(this);
	}

	public void AddItem(ItemModel itemModel)
	{
		ItemIds.Add(itemModel.Id.ToString());
	}

	public bool HasItem(ItemModel itemModel)
	{
		return ItemIds.Contains(itemModel.Id.ToString());
	}

	public void RemoveItem(ItemModel itemModel)
	{
		UnequipItem(itemModel);
		ItemIds.Remove(itemModel.Id.ToString());
	}

	public void SellItem(ItemModel itemModel, int sellPrice)
	{
		if(ItemIds.Remove(itemModel.Id.ToString()))
		{
			AddGold(sellPrice);
		}
	}

	public void UnequipItem(ItemModel itemModel)
	{
		if(itemModel.ItemType == ItemType.Small)
		{
			EquippedSmallItems.Remove(itemModel.Id.ToString());
		}
		else if(EquippedBaseSlotItems[(int)itemModel.ItemType] == itemModel.Id.ToString())
		{
			SetEquippedBaseSlotItem(itemModel.ItemType, null);
		}
	}

	public void SetName(string name)
	{
		Name = name;

		NameChangedEvent?.Invoke(this);
	}

	public void SetEquippedBaseSlotItem(ItemType itemType, ItemModel itemModel)
	{
		EquippedBaseSlotItems[(int)itemType] = itemModel?.Id.ToString();

		EquipmentChangedEvent?.Invoke(this);
	}

	public void SetEquippedSmallSlotItem(int slotIndex, ItemModel itemModel)
	{
		while(slotIndex >= EquippedSmallItems.Count - 1)
		{
			EquippedSmallItems.Add(null);
		}

		EquippedSmallItems[slotIndex] = itemModel?.Id.ToString();

		EquipmentChangedEvent?.Invoke(this);
	}

	public bool HasEquippedItem(string itemId)
	{
		foreach(string baseSlotItem in EquippedBaseSlotItems)
		{
			if(baseSlotItem == itemId)
			{
				return true;
			}
		}

		foreach(string smallItem in EquippedSmallItems)
		{
			if(smallItem == itemId)
			{
				return true;
			}
		}

		return false;
	}

	public string GetNameAndIcon(int iconSize = 30)
	{
		return $"{Name}[img={{{iconSize}}}, color=#{ClassModel.PrimaryColor.ToHtml()}]{ClassModel.IconPath}[/img]";
	}

	public string GetNameAndIcon(RichTextParameters richTextParameters)
	{
		return $"{Name}[img={{{richTextParameters.FontSize}}}, color=#{ClassModel.PrimaryColor.ToHtml()}]{ClassModel.IconPath}[/img]";
	}

	public int GetSmallItemSlotCount()
	{
		int smallItemSlotCount = (Level + 1) / 2;
		foreach(string baseSlotItem in EquippedBaseSlotItems)
		{
			if(baseSlotItem != null)
			{
				smallItemSlotCount += ModelDB.GetById<ItemModel>(baseSlotItem).SmallItemSlotCount;
			}
		}

		return smallItemSlotCount;
	}

	public void SetDonationAMDCardIds(string[] donationAMDCardIds)
	{
		DonationAMDCardIds = donationAMDCardIds;
	}

	public void AddCheckmark()
	{
		if(CheckmarkCount == 18)
		{
			// Max amount of checkmarks already earned
			return;
		}

		CheckmarkCount++;

		if(CheckmarkCount % 3 == 0)
		{
			AddAvailablePerk();
		}

		CheckmarkCountChangedEvent?.Invoke(this);
	}

	public void RemoveCheckmark()
	{
		if(CheckmarkCount % 3 == 0)
		{
			// Cannot remove checkmarks when at any threshold of getting a perk
			return;
		}

		CheckmarkCount--;
		CheckmarkCountChangedEvent?.Invoke(this);
	}

	public void AcquirePerk(int perkIndex)
	{
		if(!CanAcquirePerk(perkIndex))
		{
			return;
		}

		AcquiredPerkIndices.Add(perkIndex);

		PerksChangedEvent?.Invoke(this);
		ClassModel.Perks[perkIndex].OnPerkAcquired(this);
	}

	public int GetUsedPerkCount()
	{
		ClassModel classModel = ClassModel;

		int perkBoxCount = 0;

		foreach(int acquiredPerkIndex in AcquiredPerkIndices)
		{
			PerkModel perkModel = classModel.Perks[acquiredPerkIndex];
			perkBoxCount += perkModel.PerkBoxCount;
		}

		return perkBoxCount;
	}

	public int GetAvailablePerkCount()
	{
		int usedPerkCount = GetUsedPerkCount();
		return TotalAvailablePerkCount - usedPerkCount;
	}

	public void AddAvailablePerk()
	{
		TotalAvailablePerkCount++;
	}

	public bool GetPerkAcquired(int perkIndex)
	{
		return AcquiredPerkIndices.Contains(perkIndex);
	}

	public bool CanAcquirePerk(int perkIndex)
	{
		PerkModel perkModel = ClassModel.Perks[perkIndex];
		return GetAvailablePerkCount() >= perkModel.PerkBoxCount && !GetPerkAcquired(perkIndex);
	}

	public bool GetCanRetire(SavedCampaign savedCampaign)
	{
		return SavedPersonalQuest != null && SavedPersonalQuest.Model.GetCanRetire(savedCampaign, SavedPersonalQuest.PersonalQuestData);
	}

	public bool CanAfford(int goldCost)
	{
		return Gold >= goldCost;
	}
}