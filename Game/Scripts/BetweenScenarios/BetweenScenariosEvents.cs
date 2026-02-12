public class BetweenScenariosEvents
{
	public class CalculateBuyPrice : BetweenScenariosEvent<CalculateBuyPrice.Parameters>
	{
		public class Parameters(SavedCharacter buyer, ItemModel itemModel, int initialPrice) : ParametersBase
		{
			public SavedCharacter Buyer { get; private set; } = buyer;
			public ItemModel ItemModel { get; private set; } = itemModel;
			public int Price { get; private set; } = initialPrice;

			public void AdjustPrice(int delta)
			{
				Price += delta;
			}
		}
	}

	private readonly CalculateBuyPrice _calculateBuyPrice = new CalculateBuyPrice();
	public static CalculateBuyPrice CalculateBuyPriceEvent => BetweenScenariosController.Instance.Events._calculateBuyPrice;

	public class ItemBought : BetweenScenariosEvent<ItemBought.Parameters>
	{
		public class Parameters(SavedCharacter buyer, ItemModel itemModel, int price) : ParametersBase
		{
			public SavedCharacter Buyer { get; private set; } = buyer;
			public ItemModel ItemModel { get; private set; } = itemModel;
			public int Price { get; private set; } = price;
		}
	}

	private readonly ItemBought _itemBought = new ItemBought();
	public static ItemBought ItemBoughtEvent => BetweenScenariosController.Instance.Events._itemBought;

	public class CalculateSellPrice : BetweenScenariosEvent<CalculateSellPrice.Parameters>
	{
		public class Parameters(SavedCharacter seller, ItemModel itemModel, int initialSellPrice) : ParametersBase
		{
			public SavedCharacter Seller { get; private set; } = seller;
			public ItemModel ItemModel { get; private set; } = itemModel;
			public int SellPrice { get; private set; } = initialSellPrice;

			public void AdjustSellPrice(int delta)
			{
				SellPrice += delta;
			}
		}
	}

	private readonly CalculateSellPrice _calculateSellPrice = new CalculateSellPrice();
	public static CalculateSellPrice CalculateSellPriceEvent => BetweenScenariosController.Instance.Events._calculateSellPrice;

	public class ItemSold : BetweenScenariosEvent<ItemSold.Parameters>
	{
		public class Parameters(SavedCharacter seller, ItemModel itemModel, int sellPrice) : ParametersBase
		{
			public SavedCharacter Seller { get; private set; } = seller;
			public ItemModel ItemModel { get; private set; } = itemModel;
			public int SellPrice { get; private set; } = sellPrice;
		}
	}

	private readonly ItemSold _itemSold = new ItemSold();
	public static ItemSold ItemSoldEvent => BetweenScenariosController.Instance.Events._itemSold;

	public class DrawRoadEvent : BetweenScenariosEvent<DrawRoadEvent.Parameters>
	{
		public class Parameters() : ParametersBase
		{
			public bool DrawEvent { get; private set; } = true;

			public void SetDrawEvent(bool drawEvent)
			{
				DrawEvent = drawEvent;
			}
		}
	}

	private readonly DrawRoadEvent _drawRoadEvent = new DrawRoadEvent();
	public static DrawRoadEvent DrawRoadEventEvent => BetweenScenariosController.Instance.Events._drawRoadEvent;
}