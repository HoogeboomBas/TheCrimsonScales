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
			foreach(TextureRect itemViewTextureRect in _itemView.TextureRects)
			{
				UIHelper.SetItemMaterial(itemViewTextureRect, itemModel.ItemState);
			}

			Color modulateColor;
			switch(itemModel.ItemState)
			{
				case ItemState.Available:
					modulateColor = Colors.White;
					break;
				case ItemState.Spent:
					modulateColor = UIHelper.SpentColor;
					break;
				case ItemState.Consumed:
				case ItemState.UnrecoverablyConsumed:
					modulateColor = UIHelper.LostColor;
					break;
				case ItemState.Active:
				case ItemState.Using:
					modulateColor = UIHelper.ActiveColor;
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