using System.Collections.Generic;

public class CardElementInfusion
{
	public IReadOnlyList<Element> ConsumableElements { get; }
	public IReadOnlyList<Element> PossibleInfusedElements { get; }

	private CardElementInfusion(IReadOnlyList<Element> consumableElements, IReadOnlyList<Element> possibleInfusedElements)
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

	public static CardElementInfusion Consume(IReadOnlyList<Element> consumableElements, Element infusedElement)
	{
		return new CardElementInfusion(consumableElements, [infusedElement]);
	}

	public static CardElementInfusion ConsumeWild(Element infusedElement)
	{
		return Consume(Elements.All, infusedElement);
	}
}