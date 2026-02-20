using System.Linq;
using Godot;

public class HealDiamondPlus : HealEnhancementMark
{
	public override EnhancementModel[] PossibleEnhancements { get; } =
		Enhancements.PositiveConditions
			.Prepend(ModelDB.Enhancement<PlusOneHealEnhancement>())
			.Concat(Enhancements.Elements)
			.ToArray();

	public HealDiamondPlus(AbilityCardSideModel abilityCardSideModel, Vector2 normalizedPosition,
		EnhancementCostType enhancementCostType = EnhancementCostType.AutoDetect)
		: base(abilityCardSideModel, normalizedPosition, enhancementCostType)
	{
	}
}