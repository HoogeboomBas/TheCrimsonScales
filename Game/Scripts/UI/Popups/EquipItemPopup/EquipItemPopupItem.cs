using System;
using Godot;

public partial class EquipItemPopupItem : Control
{
	[Export]
	private ItemView _itemView;
	[Export]
	private BetterButton _betterButton;
	[Export]
	private BetterButton _sellButton;

	public ItemModel ItemModel { get; private set; }

	public event Action<EquipItemPopupItem> PressedEvent;
	public event Action<EquipItemPopupItem> SellPressedEvent;

	public void Init(ItemModel itemModel)
	{
		ItemModel = itemModel;

		_itemView.SetItem(ItemModel);

		_betterButton.Pressed += OnPressed;
		_sellButton.Pressed += OnSellClicked;
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}

	private void OnSellClicked()
	{
		SellPressedEvent?.Invoke(this);
	}
}