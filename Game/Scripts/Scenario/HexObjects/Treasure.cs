using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Treasure : LootableObject
{
	[Export]
	private Control _goalLabelContainer;

	private Character _lootingCharacter;

	private bool _lootSet;

	public int TreasureNumber { get; private set; }
	public bool Looted { get; private set; }

	private Func<Character, GDTask> _obtainLootFunction;
	private Action<Character> _scenarioEndFunction;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		if(!_lootSet)
		{
			Log.Error("Trying to initialize the treasure while the loot hasn't been set.");
			return;
		}

		if(GameController.Instance.SavedScenarioProgress.CollectedTreasureChestNumbers.Contains(TreasureNumber))
		{
			await Destroy(true);
		}
	}

	public void SetObtainLootFunction(int treasureNumber, Func<Character, GDTask> obtainLootFunction, Action<Character> scenarioEndFunction)
	{
		_lootSet = true;

		TreasureNumber = treasureNumber;

		_goalLabelContainer.SetVisible(TreasureNumber <= 0);

		_obtainLootFunction = obtainLootFunction;
		_scenarioEndFunction = scenarioEndFunction;
	}

	public void SetItemLoot(int treasureNumber, ItemModel itemModel)
	{
		SetObtainLootFunction(treasureNumber,
			async character =>
			{
				ItemModel item = itemModel.ToMutable();
				item.Init(character);
				character.AddItem(item);

				await PromptManager.Prompt(new TreasureItemRewardPrompt(_lootingCharacter, itemModel, null), _lootingCharacter);
			},
			character =>
			{
				SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
				savedItem.AddUnlocked(1);

				character.SavedCharacter.AddItem(itemModel);
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

		_scenarioEndFunction?.Invoke(_lootingCharacter);
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new GenericInfoItem.Parameters(this, "Treasure", "Great rewards lie within!"));
	}
}