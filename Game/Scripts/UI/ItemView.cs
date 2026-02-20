using System.Collections.Generic;
using Godot;

public partial class ItemView : Control
{
	[Export]
	private Control _container;

	[Export]
	public TextureRect TextureRect { get; private set; }

	[Export]
	private Label _costLabel;
	[Export]
	private Label _itemCountLabel;

	[Export]
	private PackedScene _characterTokenScene;
	[Export]
	private Control _characterTokenParent;
	[Export]
	private Control _maxUseCountTokenParent;

	[Export]
	public TextureRect[] TextureRects { get; private set; }

	private readonly List<ItemViewCharacterToken> _tokens = new List<ItemViewCharacterToken>();

	public ItemModel ItemModel { get; private set; }

	public void SetItem(ItemModel itemModel, bool showCharacterToken = false)
	{
		ItemModel = itemModel;

		foreach(ItemViewCharacterToken token in _tokens)
		{
			token.QueueFree();
		}

		_tokens.Clear();

		if(ItemModel == null)
		{
			_container.SetVisible(false);

			return;
		}

		_container.SetVisible(true);

		TextureRect.SetTexture(ItemModel.GetTexture());

		SetCost(ItemModel.Cost);
		SetItemCount(1, ItemModel.ShopCount);

		_container.SetScale(Size / _container.Size);
		this.DelayedCall(() =>
		{
			_container.SetScale(Size / _container.Size);
		});

		if(showCharacterToken)
		{
			// Add use slots
			if(ItemModel.Owner != null && ItemModel.HasUseSlots && ItemModel.UseSlotIndex < ItemModel.UseSlots.Count)
			{
				ItemUseSlot useSlot = ItemModel.UseSlots[ItemModel.UseSlotIndex];
				Texture2D tokenTexture = ItemModel.Owner.ClassModel.CharacterTokenTexture;

				ItemViewCharacterToken characterToken = _characterTokenScene.Instantiate<ItemViewCharacterToken>();
				_characterTokenParent.AddChild(characterToken);
				characterToken.Init(tokenTexture, useSlot.NormalizedPosition);
				_tokens.Add(characterToken);
			}

			// Add tokens for max use count items (like orbs)
			if(ItemModel.Owner != null && itemModel.HasMaxUseCount)
			{
				Texture2D tokenTexture = ItemModel.Owner.ClassModel.CharacterTokenTexture;

				for(int i = 0; i < itemModel.CurrentUseCountWithMaxUseCount; i++)
				{
					ItemViewCharacterToken characterToken = _characterTokenScene.Instantiate<ItemViewCharacterToken>();
					_maxUseCountTokenParent.AddChild(characterToken);
					characterToken.Init(tokenTexture, Vector2.Zero);
					_tokens.Add(characterToken);
				}
			}
		}
	}

	public void SetCost(int cost)
	{
		_costLabel.SetText(cost.ToString());
	}

	public void SetItemCount(int first, int second)
	{
		_itemCountLabel.SetText($"{first}/{second}");
	}
}