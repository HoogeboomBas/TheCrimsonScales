public class BetweenScenariosEvents
{
	public class CalculateItemBuyPrice : BetweenScenariosEvent<CalculateItemBuyPrice.Parameters>
	{
		public class Parameters(SavedCharacter buyer, ItemModel itemModel, int initialPrice) : ParametersBase
		{
			public SavedCharacter Buyer { get; } = buyer;
			public ItemModel ItemModel { get; } = itemModel;

			public int Price { get; private set; } = initialPrice;

			public void AdjustPrice(int delta)
			{
				Price += delta;
			}
		}
	}

	private readonly CalculateItemBuyPrice _calculateItemBuyPrice = new CalculateItemBuyPrice();
	public static CalculateItemBuyPrice CalculateItemBuyPriceEvent => BetweenScenariosController.Instance.Events._calculateItemBuyPrice;

	public class ItemBought : BetweenScenariosEvent<ItemBought.Parameters>
	{
		public class Parameters(SavedCharacter buyer, ItemModel itemModel, int price) : ParametersBase
		{
			public SavedCharacter Buyer { get; } = buyer;
			public ItemModel ItemModel { get; } = itemModel;
			public int Price { get; } = price;
		}
	}

	private readonly ItemBought _itemBought = new ItemBought();
	public static ItemBought ItemBoughtEvent => BetweenScenariosController.Instance.Events._itemBought;

	public class CalculateItemSellPrice : BetweenScenariosEvent<CalculateItemSellPrice.Parameters>
	{
		public class Parameters(SavedCharacter seller, ItemModel itemModel, int initialSellPrice) : ParametersBase
		{
			public SavedCharacter Seller { get; } = seller;
			public ItemModel ItemModel { get; } = itemModel;

			public int SellPrice { get; private set; } = initialSellPrice;

			public void AdjustSellPrice(int delta)
			{
				SellPrice += delta;
			}
		}
	}

	private readonly CalculateItemSellPrice _calculateItemSellPrice = new CalculateItemSellPrice();
	public static CalculateItemSellPrice CalculateItemSellPriceEvent => BetweenScenariosController.Instance.Events._calculateItemSellPrice;

	public class ItemSold : BetweenScenariosEvent<ItemSold.Parameters>
	{
		public class Parameters(SavedCharacter seller, ItemModel itemModel, int sellPrice) : ParametersBase
		{
			public SavedCharacter Seller { get; } = seller;
			public ItemModel ItemModel { get; } = itemModel;
			public int SellPrice { get; } = sellPrice;
		}
	}

	private readonly ItemSold _itemSold = new ItemSold();
	public static ItemSold ItemSoldEvent => BetweenScenariosController.Instance.Events._itemSold;

	public class CalculateEnhancementCost : BetweenScenariosEvent<CalculateEnhancementCost.Parameters>
	{
		public class Parameters(
			SavedCharacter buyer, SavedAbilityCard savedAbilityCard, EnhancementMark enhancementMark, EnhancementModel enhancementModel,
			int initialCost)
			: ParametersBase
		{
			public SavedCharacter Buyer { get; } = buyer;
			public SavedAbilityCard SavedAbilityCard { get; } = savedAbilityCard;
			public EnhancementMark EnhancementMark { get; } = enhancementMark;
			public EnhancementModel EnhancementModel { get; } = enhancementModel;

			public int Cost { get; private set; } = initialCost;

			public void AdjustCost(int delta)
			{
				Cost += delta;
			}
		}
	}

	private readonly CalculateEnhancementCost _calculateEnhancementCost = new CalculateEnhancementCost();
	public static CalculateEnhancementCost CalculateEnhancementCostEvent => BetweenScenariosController.Instance.Events._calculateEnhancementCost;

	public class EnhancementBought : BetweenScenariosEvent<EnhancementBought.Parameters>
	{
		public class Parameters(
			SavedCharacter buyer, SavedAbilityCard savedAbilityCard, EnhancementMark enhancementMark, EnhancementModel enhancementModel, int cost)
			: ParametersBase
		{
			public SavedCharacter Buyer { get; } = buyer;
			public SavedAbilityCard SavedAbilityCard { get; } = savedAbilityCard;
			public EnhancementMark EnhancementMark { get; } = enhancementMark;
			public EnhancementModel EnhancementModel { get; } = enhancementModel;
			public int Cost { get; } = cost;
		}
	}

	private readonly EnhancementBought _enhancementBought = new EnhancementBought();
	public static EnhancementBought EnhancementBoughtEvent => BetweenScenariosController.Instance.Events._enhancementBought;

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

	public class XPChanged : BetweenScenariosEvent<XPChanged.Parameters>
	{
		public class Parameters(SavedCharacter character) : ParametersBase
		{
			public SavedCharacter Character { get; } = character;
		}
	}

	private readonly XPChanged _xpChanged = new XPChanged();
	public static XPChanged XPChangedEvent => BetweenScenariosController.Instance.Events._xpChanged;
}