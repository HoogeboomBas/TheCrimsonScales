using System.Collections.Generic;
using Godot;

public partial class EquipmentPopup : Popup<EquipmentPopup.Request>
{
	public class Request : PopupRequest
	{
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private EquipmentSlot[] _baseSlots;
	[Export]
	private PackedScene _equipmentSlotScene;
	[Export]
	private Control _smallItemsParent;
	[Export]
	private ScrollContainer _scrollContainer;

	private readonly List<EquipmentSlot> _smallItemSlots = new List<EquipmentSlot>();

	public override void _Ready()
	{
		base._Ready();

		for(int i = 0; i < _baseSlots.Length; i++)
		{
			EquipmentSlot equipmentSlot = _baseSlots[i];
			equipmentSlot.Init((ItemType)i, i);
			equipmentSlot.PressedEvent += OnBaseSlotPressed;
		}
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		SavedCharacter savedCharacter = PopupRequest.SavedCharacter;

		int smallItemSlotCount = savedCharacter.GetSmallItemSlotCount();

		for(int i = 0; i < smallItemSlotCount; i++)
		{
			EquipmentSlot equipmentSlot = _equipmentSlotScene.Instantiate<EquipmentSlot>();
			_smallItemsParent.AddChild(equipmentSlot);
			equipmentSlot.Init(ItemType.Small, i);
			equipmentSlot.PressedEvent += OnSmallItemSlotPressed;
			_smallItemSlots.Add(equipmentSlot);
		}

		_scrollContainer.CustomMinimumSize = new Vector2(_scrollContainer.CustomMinimumSize.X, smallItemSlotCount >= 5 ? 420 : 400);

		savedCharacter.EquipmentChangedEvent += OnEquipmentChanged;

		OnEquipmentChanged(savedCharacter);
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(EquipmentSlot equipmentSlot in _smallItemSlots)
		{
			equipmentSlot.QueueFree();
		}

		_smallItemSlots.Clear();

		SavedCharacter savedCharacter = PopupRequest.SavedCharacter;
		savedCharacter.EquipmentChangedEvent -= OnEquipmentChanged;
	}

	private void OnBaseSlotPressed(EquipmentSlot equipment)
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new EquipItemPopup.Request
		{
			SavedCharacter = PopupRequest.SavedCharacter,
			ItemType = equipment.ItemType,
			SlotIndex = equipment.SlotIndex,
			ItemSelectedEvent = OnBaseSlotItemSelected
		});
	}

	private void OnSmallItemSlotPressed(EquipmentSlot equipment)
	{
		AppController.Instance.PopupManager.OpenPopupOnTop(new EquipItemPopup.Request
		{
			SavedCharacter = PopupRequest.SavedCharacter,
			ItemType = equipment.ItemType,
			SlotIndex = equipment.SlotIndex,
			ItemSelectedEvent = OnSmallItemSelected
		});
	}

	private void OnBaseSlotItemSelected(int slotIndex, ItemModel itemModel)
	{
		string itemId = itemModel?.Id.ToString();
		SavedCharacter savedCharacter = PopupRequest.SavedCharacter;

		if(itemId != null)
		{
			// Remove item if it is already equipped somewhere else
			for(int i = 0; i < savedCharacter.EquippedBaseSlotItems.Length; i++)
			{
				string equippedBaseSlotItem = savedCharacter.EquippedBaseSlotItems[i];
				if(equippedBaseSlotItem == itemId)
				{
					savedCharacter.SetEquippedBaseSlotItem((ItemType)i, null);
					break;
				}
			}

			for(int i = 0; i < savedCharacter.EquippedSmallItems.Count; i++)
			{
				string equippedSmallItem = savedCharacter.EquippedSmallItems[i];
				if(equippedSmallItem == itemId)
				{
					savedCharacter.SetEquippedBaseSlotItem((ItemType)i, null);
					break;
				}
			}

			// Remove one handed item if a two handed item is equipped
			if(itemModel.ItemType == ItemType.TwoHands)
			{
				string equippedOneHandItemId = savedCharacter.EquippedBaseSlotItems[(int)ItemType.OneHand];
				if(equippedOneHandItemId != null)
				{
					savedCharacter.SetEquippedBaseSlotItem(ItemType.OneHand, null);
				}
				//ItemModel equippedOneHandItemModel = ModelDB.GetById<ItemModel>(equippedOneHandItemId);
			}

			// Remove two handed item if a one handed item is equipped
			if(slotIndex == (int)ItemType.OneHand)
			{
				string equippedTwoHandsItemId = savedCharacter.EquippedBaseSlotItems[(int)ItemType.TwoHands];
				if(equippedTwoHandsItemId != null)
				{
					ItemModel equippedTwoHandsItemModel = ModelDB.GetById<ItemModel>(equippedTwoHandsItemId);
					if(equippedTwoHandsItemModel.ItemType == ItemType.TwoHands)
					{
						savedCharacter.SetEquippedBaseSlotItem(ItemType.TwoHands, null);
					}
				}
			}
		}

		savedCharacter.SetEquippedBaseSlotItem((ItemType)slotIndex, itemModel);

		int newSmallItemSlotCount = savedCharacter.GetSmallItemSlotCount();

		if(_smallItemSlots.Count != newSmallItemSlotCount) {
			// Remove excess small item slots
			for(int i = _smallItemSlots.Count - 1; i >= newSmallItemSlotCount; i--)
			{
				savedCharacter.SetEquippedSmallSlotItem(i, null);

				EquipmentSlot equipmentSlotToRemove = _smallItemSlots[i];
				_smallItemSlots.Remove(equipmentSlotToRemove);
				_smallItemsParent.RemoveChild(equipmentSlotToRemove);
				equipmentSlotToRemove.QueueFree();
			}
			// Add missing small item slots
			for(int i = _smallItemSlots.Count; i < newSmallItemSlotCount; i++)
			{
				EquipmentSlot equipmentSlot = _equipmentSlotScene.Instantiate<EquipmentSlot>();
				_smallItemsParent.AddChild(equipmentSlot);
				equipmentSlot.Init(ItemType.Small, i);
				equipmentSlot.PressedEvent += OnSmallItemSlotPressed;
				_smallItemSlots.Add(equipmentSlot);

				savedCharacter.SetEquippedSmallSlotItem(i, null);
			}
		}
	}

	private void OnSmallItemSelected(int slotIndex, ItemModel itemModel)
	{
		string itemId = itemModel?.Id.ToString();
		SavedCharacter savedCharacter = PopupRequest.SavedCharacter;

		if(itemId != null)
		{
			for(int i = 0; i < savedCharacter.EquippedSmallItems.Count; i++)
			{
				string equippedSmallItem = savedCharacter.EquippedSmallItems[i];
				if(equippedSmallItem == itemId)
				{
					savedCharacter.SetEquippedBaseSlotItem((ItemType)i, null);
					break;
				}
			}
		}

		savedCharacter.SetEquippedSmallSlotItem(slotIndex, itemModel);
	}

	private void OnEquipmentChanged(SavedCharacter savedCharacter)
	{
		for(int i = 0; i < _baseSlots.Length; i++)
		{
			EquipmentSlot baseSlot = _baseSlots[i];
			baseSlot.SetItem(ModelDB.GetById<ItemModel>(savedCharacter.EquippedBaseSlotItems[i]));
		}

		for(int i = 0; i < _smallItemSlots.Count; i++)
		{
			EquipmentSlot smallItemSlot = _smallItemSlots[i];
			string modelId = savedCharacter.EquippedSmallItems.Count > i ? savedCharacter.EquippedSmallItems[i] : null;
			smallItemSlot.SetItem(ModelDB.GetById<ItemModel>(modelId));
		}
	}
}