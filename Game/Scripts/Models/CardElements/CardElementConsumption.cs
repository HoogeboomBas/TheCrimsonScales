using System.Collections.Generic;

public class CardElementConsumption
{
	public IReadOnlyCollection<Element> ConsumableElements { get; }

	private CardElementConsumption(IReadOnlyCollection<Element> consumableElements)
	{
		ConsumableElements = consumableElements;
	}

	public static CardElementConsumption Consume(Element consumableElement)
	{
		return new CardElementConsumption([consumableElement]);
	}

	public static CardElementConsumption Consume(IReadOnlyCollection<Element> consumableElements)
	{
		return new CardElementConsumption(consumableElements);
	}

	public static CardElementConsumption ConsumeWild()
	{
		return Consume(Elements.All);
	}
}