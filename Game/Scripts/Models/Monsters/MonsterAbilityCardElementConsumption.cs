using System.Collections.Generic;

public class MonsterAbilityCardElementConsumption
{
	public IReadOnlyCollection<Element> ConsumableElements { get; }

	private MonsterAbilityCardElementConsumption(IReadOnlyCollection<Element> consumableElements)
	{
		ConsumableElements = consumableElements;
	}

	public static MonsterAbilityCardElementConsumption Consume(Element consumableElement)
	{
		return new MonsterAbilityCardElementConsumption([consumableElement]);
	}

	public static MonsterAbilityCardElementConsumption Consume(IReadOnlyCollection<Element> consumableElements)
	{
		return new MonsterAbilityCardElementConsumption(consumableElements);
	}

	public static MonsterAbilityCardElementConsumption ConsumeWild()
	{
		return Consume(Elements.All);
	}
}