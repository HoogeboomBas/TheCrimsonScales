using System.Collections.Generic;

public class CardElementInfusion
{
	public IReadOnlyCollection<Element> ConsumableElements { get; }
	public IReadOnlyCollection<Element> PossibleInfusedElements { get; }

	private CardElementInfusion(IReadOnlyCollection<Element> consumableElements, IReadOnlyCollection<Element> possibleInfusedElements)
	{
		ConsumableElements = consumableElements;
		PossibleInfusedElements = possibleInfusedElements;
	}

	public static CardElementInfusion Infuse(Element infusedElement)
	{
		return new CardElementInfusion(null, [infusedElement]);
	}

	public static CardElementInfusion InfuseWild()
	{
		return new CardElementInfusion(null, Elements.All);
	}

	public static CardElementInfusion Consume(IReadOnlyCollection<Element> consumableElements, Element infusedElement)
	{
		return new CardElementInfusion(consumableElements, [infusedElement]);
	}

	public static CardElementInfusion ConsumeWild(Element infusedElement)
	{
		return Consume(Elements.All, infusedElement);
	}
}