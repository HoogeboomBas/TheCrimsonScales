using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Treasure : LootableObject, IEventSubscriber
{
	[Export]
	public int TreasureNumber = -1;

	private Character _lootingCharacter;
	private Func<Figure, bool> _canLootFunction;
	private Func<Character, GDTask> _obtainLootFunction;

	public bool Looted { get; private set; }

	public bool IsGoal => TreasureNumber <= 0;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		if(GameController.Instance.SavedScenarioProgress.CollectedTreasureChestNumbers.Contains(TreasureNumber))
		{
			await Destroy(true);
		}

		if(IsGoal)
		{
			ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Subscribe(this,
				parameters => parameters.HexObject == this,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => "This treasure tile is a Goal tile, see special rules."));
				}
			);
		}
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Unsubscribe(this);
	}

	public void SetObtainLootFunction(Func<Character, GDTask> obtainLootFunction)
	{
		_obtainLootFunction = obtainLootFunction;
	}

	public void SetItemLoot(ItemModel itemModel)
	{
		SetObtainLootFunction(async character =>
			{
				await AbilityCmd.PermanentlyGiveItem(character, itemModel);
			}
		);
	}

	public void SetItemDesignLoot(ItemModel itemModel)
	{
		SetObtainLootFunction(async character =>
			{
				await AbilityCmd.GainItemDesign(character, itemModel);
			}
		);
	}

	public override bool CanLoot(Figure lootObtainer)
	{
		return base.CanLoot(lootObtainer) && lootObtainer is Character && (_canLootFunction == null || _canLootFunction(lootObtainer));
	}

	public void SetCanLootFunction(Func<Figure, bool> canLootFunction)
	{
		_canLootFunction = canLootFunction;
	}

	public override async GDTask Loot(Figure lootObtainer)
	{
		AppController.Instance.AudioController.PlayFastForwardable("res://Audio/SFX/Chest Open 1.wav", delay: 0.3f);

		await base.Loot(lootObtainer);

		Looted = true;
		_lootingCharacter = (Character)lootObtainer;

		if(_obtainLootFunction != null)
		{
			await _obtainLootFunction.Invoke(_lootingCharacter);
		}

		GameController.Instance.EndEvent += OnScenarioEnd;
	}

	private void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
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