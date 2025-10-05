using System;
using Godot;

public partial class PartyInfoCharacterItem : Control
{
	[Export]
	private ItemView _itemView;
	[Export]
	private TextureRect _itemTypeIcon;
	[Export]
	private Panel _border;

	public void Init(ItemType itemType, ItemModel itemModel)
	{
		_itemView.SetItem(itemModel);
		_itemView.SetVisible(itemModel != null);

		if(itemModel != null)
		{
			UIHelper.SetItemMaterial(_itemView.TextureRect, itemModel.ItemState);

			Color modulateColor = Colors.White;
			switch(itemModel.ItemState)
			{
				case ItemState.Available:
					modulateColor = Colors.White;
					break;
				case ItemState.Spent:
					modulateColor = UIHelper.SpentColor;
					break;
				case ItemState.Consumed:
					modulateColor = UIHelper.LostColor;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			_border.SetSelfModulate(modulateColor);
		}

		_itemTypeIcon.SetTexture(ResourceLoader.Load<Texture2D>(Icons.GetItem(itemType)));
		_itemTypeIcon.SetVisible(itemModel == null);

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		if(_itemView.ItemModel != null)
		{
			AppController.Instance.ItemPreview.Focus(this, _itemView.ItemModel);
		}
	}

	private void OnMouseExited()
	{
		AppController.Instance.ItemPreview.Unfocus(this);
	}
}