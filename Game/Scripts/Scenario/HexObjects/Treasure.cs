using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Treasure : LootableObject
{
	[Export]
	public int TreasureNumber = -1;

	private Character _lootingCharacter;

	public bool Looted { get; private set; }

	private Func<Character, GDTask> _obtainLootFunction;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		if(GameController.Instance.SavedScenarioProgress.CollectedTreasureChestNumbers.Contains(TreasureNumber))
		{
			await Destroy(true);
		}
	}

	public void SetObtainLootFunction(Func<Character, GDTask> obtainLootFunction)
	{
		_obtainLootFunction = obtainLootFunction;
	}

	public void SetItemLoot(ItemModel itemModel)
	{
		SetObtainLootFunction(
			async character =>
			{
				await AbilityCmd.PermanentlyGiveItem(character, itemModel);
			}
		);
	}

	public override bool CanLoot(Figure lootObtainer)
	{
		return base.CanLoot(lootObtainer) && lootObtainer is Character;
	}

	public override async GDTask Loot(Figure lootObtainer)
	{
		AppController.Instance.AudioController.PlayFastForwardable("res://Audio/SFX/Chest Open 1.wav", delay: 0.3f);

		await base.Loot(lootObtainer);

		Looted = true;
		_lootingCharacter = (Character)lootObtainer;

		await _obtainLootFunction.Invoke(_lootingCharacter);

		GameController.Instance.EndEvent += OnScenarioEnd;
	}

	private void OnScenarioEnd(bool backToTown, bool won, SavedScenarioProgress savedScenarioProgress)
	{
		if(TreasureNumber > 0)
		{
			savedScenarioProgress.CollectedTreasureChestNumbers.AddIfNew(TreasureNumber);
		}
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, "Treasure", "Great rewards lie within!"));
	}
}