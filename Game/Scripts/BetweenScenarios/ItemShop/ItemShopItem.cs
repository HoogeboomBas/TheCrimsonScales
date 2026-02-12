using Godot;

public partial class ItemShopItem : Control
{
	[Export]
	private ItemView _itemView;
	[Export]
	private BetterButton _betterButton;
	[Export]
	private Label _stockLabel;

	private SavedCampaign _savedCampaign;

	public ItemModel ItemModel { get; private set; }
	public SavedItem SavedItem { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		_betterButton.Pressed += OnPressed;
	}

	public void Init(ItemModel itemModel)
	{
		ItemModel = itemModel;
		_savedCampaign = BetweenScenariosController.Instance.SavedCampaign;
		SavedItem = _savedCampaign.SavedItems[ItemModel.Id.ToString()];

		_itemView.SetItem(itemModel);

		SavedItem.StockCountChangedEvent += OnStockCountChanged;

		foreach(SavedCharacter savedCharacter in _savedCampaign.Characters)
		{
			savedCharacter.GoldChangedEvent += OnGoldChanged;
		}

		BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortraitChangedEvent += OnSelectedPortraitChanged;

		UpdateVisuals();
	}

	// public override void _ExitTree()
	// {
	// 	base._ExitTree();
	//
	// 	if(SavedItem != null)
	// 	{
	// 		SavedItem.StockCountChangedEvent -= OnStockCountChanged;
	// 	}
	//
	// 	foreach(SavedCharacter savedCharacter in _savedCampaign.Characters)
	// 	{
	// 		savedCharacter.GoldChangedEvent -= OnGoldChanged;
	// 	}
	// }

	public override void _Notification(int what)
	{
		base._Notification(what);

		if(what == NotificationPredelete && AppController.Instance != null)
		{
			if(SavedItem != null)
			{
				SavedItem.StockCountChangedEvent -= OnStockCountChanged;
			}

			foreach(SavedCharacter savedCharacter in _savedCampaign.Characters)
			{
				savedCharacter.GoldChangedEvent -= OnGoldChanged;
			}

			if(BetweenScenariosController.Instance != null)
			{
				BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortraitChangedEvent -= OnSelectedPortraitChanged;
			}
		}
	}

	private void UpdateVisuals()
	{
		SavedCharacter selectedCharacter = BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortrait?.SavedCharacter;
		bool hasItem = selectedCharacter != null && selectedCharacter.ItemIds.Contains(ItemModel.Id.ToString());

		_betterButton.SetEnabled(!hasItem && SavedItem.StockCount > 0);

		_stockLabel.Text = $"{SavedItem.StockCount} / {SavedItem.UnlockedCount}";

		_itemView.TextureRect.SetInstanceShaderParameter("grayscaleFactor", hasItem || SavedItem.StockCount == 0 ? 1f : 0f);
	}

	private bool GetCanAfford()
	{
		SavedCharacter selectedCharacter = BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortrait?.SavedCharacter;

		return
			selectedCharacter != null &&
			selectedCharacter.Gold >= BetweenScenariosController.Instance.ItemShop.GetBuyPrice(selectedCharacter, ItemModel);
	}

	private void OnPressed()
	{
		if(BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortrait == null)
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("No character selected",
				"You need to select a character first.\n"));
			return;
		}

		if(SavedItem.StockCount == 0)
		{
			return;
		}

		SavedCharacter selectedCharacter = BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortrait.SavedCharacter;

		if(!GetCanAfford())
		{
			AppController.Instance.PopupManager.RequestPopup(new TextPopup.Request("Not enough gold",
				$"{selectedCharacter.GetNameAndIcon()} does not have enough gold to buy this item.\n"));
			return;
		}

		AppController.Instance.PopupManager.RequestPopup(new BuyItemPopup.Request
		{
			ItemModel = ItemModel,
			SavedItem = SavedItem,
			Buyer = selectedCharacter,
			Price = BetweenScenariosController.Instance.ItemShop.GetBuyPrice(selectedCharacter, ItemModel)
		});

		UpdateVisuals();
	}

	private void OnStockCountChanged(SavedItem savedItem)
	{
		UpdateVisuals();
	}

	private void OnGoldChanged(SavedCharacter savedCharacter)
	{
		UpdateVisuals();
	}

	private void OnSelectedPortraitChanged(BetweenScenariosCharacterPortrait portrait)
	{
		UpdateVisuals();
	}
}